using System;
using System.Collections;
using System.Collections.Generic;

namespace ShipData
{
    public class Parts
    {
        public enum partTypes { engine, shield, weapon, armour }
        
        private event EventHandler _partStatChanged;
        private event EventHandler _partRemoved;
        private Func<int, int, Parts> _GetPArtAtPosition;

        private partTypes _type;
        public partTypes GetPartType { get { return _type; } }
        private float _amount;
        public float GetPartAmount { get { return _amount; } }
        private float _value;
        public float GetPartValue { get { return _value; } }


        // Col + Row + S = 0
        // S = -(col + row) 
        // Neighbours:   
        // N:       0   , -1   
        // NEast:  +1   , -1   
        // NWest:  -1   ,  0   
        // S:       0   , +1    
        // SEast:  +1   ,  0   
        // SWest:  -1   , +1

        public bool AssignedPos { get; private set; }
        private int _column;
        public int Column { get { return _column; } }
        private int _row;
        public int Row { get { return _row; } }
        private int _s;
        public int S { get { return _s; } }

        private float HP_Max;
        private float HP_Current;

        //construct and registering
        public Parts(float maxHP, partTypes type, float amount, float value)
        {
            HP_Max = maxHP;
            HP_Current = maxHP;
            _type = type;
            _amount = amount;
            _value = value;
            AssignedPos = false;
        }
        public void AssignPosition(int col, int row)
        {
            _column = col;
            _row = row;
            _s = -(_column + _row);
            AssignedPos = true;
        }
        public void AssignPosition(intPos2 pos)
        {
            _column = pos.Column;
            _row = pos.Row;
            _s = -(_column + _row);
            AssignedPos = true;
        }
        public void RegesterEvents(EventHandler onPartStatChange, EventHandler onPartRemove, Func<int, int, Parts> getPartAtPosition)
        {
            _partStatChanged = onPartStatChange;
            _partRemoved = onPartRemove;
            _GetPArtAtPosition = getPartAtPosition;
        }

        //damage
        public void TakeDamage(float damage)
        {
            HP_Current -= damage;

            if (HP_Current <= 0)
            {
                _partRemoved(this, EventArgs.Empty);
            }
        }
        public void ForceRemove()
        {
            _partStatChanged = null;
            _partRemoved = null;
            _GetPArtAtPosition = null;
            AssignedPos = false;
            _column = 0;
            _row = 0;
            _s = 0;
        }



        public Parts GetNeighbour_North()
        {
            return (_GetPArtAtPosition(_column, _row -1));
        }
        public Parts GetNeighbour_NEast()
        {
            return (_GetPArtAtPosition(_column + 1, _row - 1));
        }
        public Parts GetNeighbour_NWest()
        {
            return (_GetPArtAtPosition(_column - 1, _row));
        }
        public Parts GetNeighbour_South()
        {
            return (_GetPArtAtPosition(_column, _row + 1));
        }
        public Parts GetNeighbour_SEast()
        {
            return (_GetPArtAtPosition(_column + 1, _row));
        }
        public Parts GetNeighbour_SWest()
        {
            return (_GetPArtAtPosition(_column - 1, _row + 1));
        }
        public Parts[] GetNeighbour_All()
        {
            return new Parts[] {
                GetNeighbour_North(),
                GetNeighbour_NEast(),
                GetNeighbour_NWest(),
                GetNeighbour_South(),
                GetNeighbour_SEast(),
                GetNeighbour_SWest()
            } ;
        }


        //Static functions
        private static Random rnd = new Random();
        public static partTypes RandomPartType ()
        {
            var sausage = Enum.GetValues(typeof(partTypes));
            return (partTypes) sausage.GetValue(rnd.Next(sausage.Length));
        }
        


    }
}
