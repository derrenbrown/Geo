﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Geo.Interfaces;

namespace Geo.Geometries
{
    public class CoordinateSequence : IEnumerable<Coordinate>, IWktPart
    {
        private readonly List<Coordinate> _coordinates;

        public CoordinateSequence()
        {
            _coordinates = new List<Coordinate>();
        }

        public CoordinateSequence(IEnumerable<Coordinate> coordinates)
        {
            _coordinates = new List<Coordinate>(coordinates);
        }

        public IEnumerator<Coordinate> GetEnumerator()
        {
            return _coordinates.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public Coordinate this[int index]
        {
            get { return _coordinates[index]; }
        }

        public void Add(Coordinate coordinate)
        {
            _coordinates.Add(coordinate);
        }

        public void AddRange(IEnumerable<Coordinate> coordinates)
        {
            _coordinates.AddRange(coordinates);
        }

        public bool IsEmpty
        {
            get { return _coordinates.Count == 0; }
        }

        public int Count
        {
            get { return _coordinates.Count; }
        }

        public bool IsClosed()
        {
            return _coordinates.Count > 1 && _coordinates[0].Equals(_coordinates[_coordinates.Count - 1]);
        }

        protected bool Equals(CoordinateSequence other)
        {
            if (other == null)
                return false;

            if (_coordinates.Count != other._coordinates.Count)
                return false;

            return !_coordinates
                .Where((t, i) => !t.Equals(other._coordinates[i]))
                .Any();
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((CoordinateSequence) obj);
        }

        public override int GetHashCode()
        {
            return _coordinates
                .Select(x => x.GetHashCode())
                .Aggregate(0, (current, result) => (current * 397) ^ result);
        }

        public string ToWktPartString()
        {
            var buf = new StringBuilder();
            if (IsEmpty)
                buf.Append("EMPTY");
            else
            {
                buf.Append("(");
                var parts = _coordinates.Select(x => x.ToWktPartString()).ToList();
                string last = null;
                foreach (var part in parts)
                {
                    if (last == null || part != last)
                    {
                        if (last !=null)
                            buf.Append(", ");
                        last = part;
                        buf.Append(part);
                    }
                }
                buf.Append(")");
            }
            return buf.ToString();
        }

        public IEnumerable<LineSegment> ToLineSegments()
        {
            Coordinate last = null;
            foreach (var coordinate in _coordinates)
            {
                if (last != null)
                    yield return new LineSegment(last, coordinate);
                last = coordinate;
            }
        }

        public LineString ToLineString()
        {
            return new LineString(_coordinates);
        }

        public LinearRing ToLinearRing()
        {
            return new LinearRing(_coordinates);
        }

        public static bool operator ==(CoordinateSequence left, CoordinateSequence right)
        {
            return !ReferenceEquals(left, null) && !ReferenceEquals(right, null) && left.Equals(right);
        }

        public static bool operator !=(CoordinateSequence left, CoordinateSequence right)
        {
            return ReferenceEquals(left, null) || ReferenceEquals(right, null) || !left.Equals(right);
        }
    }
}
