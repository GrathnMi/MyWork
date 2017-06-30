using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;  //testing only, want this to be a pure c# class and nothing to do with unity.



namespace ShipData
{
    /// <summary>
    /// this class holds all information relivent to the "ship". 
    /// This class is only ment to hold and prosess data; 
    /// its not ment to have an update function and should aim to be as self containd as possible,
    /// using actions/events to interact with the shipManager/unity.
    /// </summary>
    public class Ship
    {
        private static System.Random rnd = new System.Random();     //for testing only

        //parts
        private List<Parts> _shipParts;
        public float GetPartCount { get { return _shipParts.Count; } }
        private Parts[,] _partsByPosition = new Parts[200,200];
        private Parts _centralPart;

        //stats
        private Dictionary<Parts.partTypes, float> _shipStats;
        public event EventHandler ShipStatChanged;
        public event EventHandler ShipDestroyedEvent;
        private Action<Parts> _createVisuels;
        private Action<Parts> _removeVisuels;

        //Constructor
        public Ship(EventHandler onShipStatChange, EventHandler onShipDestroy, Action<Parts> createVisuels, Action<Parts> removeVisuel)
        {
            ShipStatChanged = onShipStatChange;
            ShipDestroyedEvent = onShipDestroy;
            _shipParts = new List<Parts>();
            _createVisuels = createVisuels;
            _removeVisuels = removeVisuel;
        }
        
        //Events
        protected void OnPartRemoveEvent(object s, EventArgs e)
        {
            Parts part = (Parts)s;

            _removeVisuels(part);
            _shipParts.Remove(part);
            RemovePartAtPosition(part.Column, part.Row);

            CheckPartsConnectedToCentral();
            CheckShipDestroyCriteria();
            RecalculateShipStats();
        }
        protected void OnPartChangeEvent(object s, EventArgs e)
        {
            RecalculateShipStats();
        }


        //parts
        public void AddParts(List<Parts> newParts)
        {
            foreach (var part in newParts)
            {
                part.RegesterEvents(OnPartChangeEvent, OnPartRemoveEvent, GetPartAtPosition);

                if (part.AssignedPos == false)
                {
                    part.AssignPosition(BuildShip(part));
                }
                SetPartToPosition(part);
                _createVisuels(part);

                if (_centralPart == null)
                    _centralPart = part;
            }

            _shipParts.AddRange(newParts);
            
            RecalculateShipStats();
        }
        public void AddParts(Parts newPart)
        {
            newPart.RegesterEvents(OnPartChangeEvent, OnPartRemoveEvent, GetPartAtPosition);

            if (newPart.AssignedPos == false)
            {
                newPart.AssignPosition(BuildShip(newPart));
            }
            SetPartToPosition(newPart);

            _createVisuels(newPart);
            _shipParts.Add(newPart);
            
            RecalculateShipStats();
        }

        public List<Parts> GetShipWeapons
        {
            get {
                List<Parts> sausages = new List<Parts>();
                foreach (var part in _shipParts)
                {
                    if (part.GetPartType == Parts.partTypes.weapon)
                        sausages.Add(part);
                }
                return sausages;
            }
        }
        protected Parts GetPartAtPosition(int column, int row)
        {
            //we are adding 100 to avoid negative numbers. see SetPartToPosition function.
            Parts part = null;
            part = _partsByPosition[column + 100, row + 100];

            return part;
        }
        private void SetPartToPosition(Parts part)
        {
            //TODO: currently we are adding +100 to avoid going into - index's in the array.
            //add modulow (%?) to make the array loop around insted of just adding 100.
            _partsByPosition[part.Column + 100, part.Row + 100] = part;
        }
        private void RemovePartAtPosition(int column, int row)
        {
            //we are adding 100 to avoid negative numbers. see SetPartToPosition function.
            _partsByPosition[column + 100, row + 100] = null;
            
        }
        /// <summary>
        /// Removes parts without the part being involved (stopping call backs).
        /// </summary>
        /// <param name="parts"></param>
        private void ForceRemoveParts(List<Parts> parts)
        {
            foreach (Parts part in parts)
            {
                _shipParts.Remove(part);
                _removeVisuels(part);
                RemovePartAtPosition(part.Column, part.Row);
                part.ForceRemove();
            }
            RecalculateShipStats();
        }
        
        //ShipBuilding
        private intPos2 BuildShip(Parts part)
        {
            //TODO: this needs alot of work. function is to long, needs splitting and optimization.
            Dictionary<intPos2, int> emptyNeighbours = new Dictionary<intPos2, int>(new Utilities.intPos2EqualityComparer());
            List<intPos2> exspantionPoints_2plus = new List<intPos2>();

            //each hex
            foreach (Parts shipPart in _shipParts)
            {
                //check each neighbour to see if its null.

                //north         0   ,   -1
                if (GetPartAtPosition(shipPart.Column, shipPart.Row - 1) == null)
                {
                    //found an empty!
                    intPos2 colRow = new intPos2(shipPart.Column, shipPart.Row - 1);
                    if (emptyNeighbours.ContainsKey(colRow))
                    {
                        emptyNeighbours[colRow] += 1;
                    }
                    else
                    {
                        emptyNeighbours.Add(colRow, 1);
                    }
                }
                //north-East    +1   ,   -1
                if (GetPartAtPosition(shipPart.Column + 1, shipPart.Row - 1) == null)
                {
                    //found an empty!
                    intPos2 colRow = new intPos2(shipPart.Column + 1, shipPart.Row - 1);
                    //has it been found before?
                    if (emptyNeighbours.ContainsKey(colRow))
                    {
                        emptyNeighbours[colRow] += 1;
                    }
                    else
                    {
                        emptyNeighbours.Add(colRow, 1);
                    }
                }
                //north-West   -1   ,   0
                if (GetPartAtPosition(shipPart.Column - 1, shipPart.Row) == null)
                {
                    //found an empty!
                    intPos2 colRow = new intPos2(shipPart.Column - 1, shipPart.Row);
                    if (emptyNeighbours.ContainsKey(colRow))
                    {
                        emptyNeighbours[colRow] += 1;
                    }
                    else
                    {
                        emptyNeighbours.Add(colRow, 1);
                    }
                }
                //south         0   ,  +1
                if (GetPartAtPosition(shipPart.Column, shipPart.Row + 1) == null)
                {
                    //found an empty!
                    intPos2 colRow = new intPos2(shipPart.Column, shipPart.Row + 1);
                    if (emptyNeighbours.ContainsKey(colRow))
                    {
                        emptyNeighbours[colRow] += 1;
                    }
                    else
                    {
                        emptyNeighbours.Add(colRow, 1);
                    }
                }
                //south-East    +1   ,  0
                if (GetPartAtPosition(shipPart.Column + 1, shipPart.Row) == null)
                {
                    //found an empty!
                    intPos2 colRow = new intPos2(shipPart.Column + 1, shipPart.Row);
                    if (emptyNeighbours.ContainsKey(colRow))
                    {
                        emptyNeighbours[colRow] += 1;
                    }
                    else
                    {
                        emptyNeighbours.Add(colRow, 1);
                    }
                }
                //south-West   -1   ,   +1
                if (GetPartAtPosition(shipPart.Column - 1, shipPart.Row + 1) == null)
                {
                    //found an empty!
                    intPos2 colRow = new intPos2(shipPart.Column - 1, shipPart.Row + 1);
                    if (emptyNeighbours.ContainsKey(colRow))
                    {
                        emptyNeighbours[colRow] += 1;
                    }
                    else
                    {
                        emptyNeighbours.Add(colRow, 1);
                    }
                }
            }
            /*
            foreach (intPos2 item in emptyNeighbours.Keys)
            {
                Debug.Log(item.Column.ToString() + " " + item.Row.ToString() + ". seen:" + emptyNeighbours[item].ToString());
            }
            */

            //if a possible location only has 1 neighbour then its not a valid exspantion point.
            foreach (intPos2 colRow in emptyNeighbours.Keys)
            {
                if (emptyNeighbours[colRow] > 1)
                {
                    exspantionPoints_2plus.Add(colRow);
                }
            }
            //Debug.Log("All Valid Exspantion Points Complete. " + exspantionPoints_2plus.Count.ToString() + " possible points were found.");
            
            int sausages = rnd.Next(0, exspantionPoints_2plus.Count);
            return exspantionPoints_2plus[sausages];
        }






        private void CheckPartsConnectedToCentral()
        {
            //finds all connected parts to the central point.
            

            Queue<Parts> partsToCheck = new Queue<Parts>();
            List<Parts> connectedParts = new List<Parts>();
            connectedParts.Add(_centralPart);

            partsToCheck.Enqueue(_centralPart);

            //while there are parts to check
            while (partsToCheck.Count > 0)
            {
                Parts p = partsToCheck.Dequeue();

                //get all the neighbours...
                Parts[] pNeighbours = p.GetNeighbour_All();
                // look though all the neighbours to see if they are null (an edge)
                // or if they have been checked before.
                foreach (Parts p2 in pNeighbours)
                {
                    if (p2 != null && connectedParts.Contains(p2) == false )
                    {
                        connectedParts.Add(p2);
                        partsToCheck.Enqueue(p2);
                    }
                }
            }
            if(connectedParts.Count == _shipParts.Count)
            {
                //Debug.Log("no parts need removing");
                return;
            }

            //Debug.Log("Created connected list, Sorting parts into keep / remove lists");

            //TODO: this can be optimized.
            List<Parts> partKeep = new List<Parts>();
            List<Parts> partRemove = new List<Parts>();
            foreach (Parts p in _shipParts)
            {
                if (connectedParts.Contains(p))
                {
                    partKeep.Add(p);
                }
                else
                    partRemove.Add(p);
            }

            //Debug.Log("calling remove function with: " + sausagesRemove.Count.ToString() + " parts to remove.");
            ForceRemoveParts( partRemove );

            //_shipParts is modifyed from the parts being removed.
        }
        private void CheckShipDestroyCriteria()
        {
            if (_shipParts.Count > 2)
                return;
            


            ShipDestroyedEvent(this, EventArgs.Empty);
        }

        //Damage
        public void TakeDamage(float damage, intPos2 collisionPoint)
        {
            Parts hitpart = GetPartAtPosition(collisionPoint.Column, collisionPoint.Row);
            //TODO: can i do this better? :S
            _shipParts[_shipParts.IndexOf(hitpart)].TakeDamage(damage);

            CheckShipDestroyCriteria();
        }
        
        //Ship Stats
        private void RecalculateShipStats()
        {
            //Wipe old stats.
            _shipStats = new Dictionary<Parts.partTypes, float>();
            //Combines ship parts to create the ships stats.
            foreach (var part in _shipParts)
            {
                if (_shipStats.ContainsKey(part.GetPartType))
                {
                    _shipStats[part.GetPartType] += part.GetPartAmount;
                }
                else
                {
                    _shipStats.Add(part.GetPartType, part.GetPartAmount);
                }
            }
           
            //Call the ShipChanged Event;
            ShipStatChanged(this, EventArgs.Empty);     //this is needed for the object to be created so should never be null.
        }
        public float GetStatOfType(Parts.partTypes type)
        {
            float stats = 0.0f;

            if (_shipStats.TryGetValue(type, out stats))
            {
                return stats;
            }
            else
                return 0.0f;
        }
    }
}

