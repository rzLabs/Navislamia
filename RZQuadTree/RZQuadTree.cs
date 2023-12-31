
using System.Drawing;

namespace RZQuadTree
{
    public class RZQuadTree<T>
    {
        QuadNode<T> _rootNode;

        public RZQuadTree(int x, int y, int width, int height, IQuadTreeObjectArea<T> objectArea, int depth, int maxChildrenPerNode)
        {
            _rootNode = new QuadNode<T>(x, y, width, height, objectArea, depth, 0, maxChildrenPerNode);
        }
    }

    /// <summary>
    /// QuadNode implementation based on <see cref="https://github.com/Auios/Auios.QuadTree/blob/main/Auios.QuadTree/QuadTree.cs"/> by Auios
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public struct QuadNode<T>
    {
        private readonly int _maxDepth;
        private readonly int _maxChildrenPerNode;

        // increased every time the node is quarter
        private int _currentDepth;

        // area this node occupies
        private readonly QuadRect _area;

        // objects within this branch
        private readonly HashSet<T> _objects = new HashSet<T>();

        // interface for getting an objects area
        private readonly IQuadTreeObjectArea<T> _objectArea;

        // children of this node
        private Dictionary<int, QuadNode<T>> _nodes = new Dictionary<int, QuadNode<T>>();

        // has this node been quartered
        private bool _hasChildren => _nodes.Count > 0;

        public QuadNode(int x, int y, int width, int height, IQuadTreeObjectArea<T> objectArea, int maxDepth = 10, int currentDepth = 0, int maxChildrenPerNode = 10)
        {
            _area = new QuadRect(x, y, width, height);
            _objectArea = objectArea;
            _maxDepth = maxDepth;
            _currentDepth = currentDepth;
            _maxChildrenPerNode = maxChildrenPerNode;
        }

        /// <summary>
        /// Insert an object into this node (if branch) or one of its children respectively if this node has been quartered
        /// </summary>
        /// <param name="obj">Object to be inserted</param>
        /// <returns>True or False</returns>
        public bool Insert(T obj)
        {
            if (!CanAdd(obj))
                return false;

            if (_hasChildren)
            {
                for (int i = 0; i < _nodes.Count; i++)
                {
                    if (_nodes[i].Insert(obj))
                        return true;
                }
            }

            _objects.Add(obj);

            if (_objects.Count > _maxChildrenPerNode)
            {
                Quarter();
            }

            return true;
        }

        /// <summary>
        /// Find objects in this node that exist inside of the provided area
        /// </summary>
        /// <param name="rect">Area rectangle to be checked for objects</param>
        /// <returns></returns>
        public List<T> FindObjects(QuadRect rect)
        {
            var objects = new List<T>();

            if (_hasChildren)
            {
                for (int i = 0; i < _nodes.Count; i++)
                {
                    objects.AddRange(_nodes[i].FindObjects(rect));
                }

                return objects;
            }

            if (IsOverlapping(rect))
            {
                objects.AddRange(_objects);
            }

            return objects;
        }

        /// <summary>
        /// Remove an object from this node
        /// </summary>
        /// <param name="obj">Object to be removed</param>
        public void Remove(T obj)
        {
            if (_hasChildren)
            {
                for (int i = 0; i < _nodes.Count; i++)
                {
                    _nodes[i].Remove(obj);
                }

                return;
            }

            _objects.Remove(obj);

            Clean();         
        }

        /// <summary>
        /// Reset this node to an unquarted state if its children nodes no longer contain any objects
        /// </summary>
        private void Clean()
        {
            if (_hasChildren)
            {
                for (int i = 0; i < _nodes.Count; i++)
                {
                    // If any children nodes contain objects, return out of the clean
                    if (_nodes[i]._objects.Count > 0)
                        return;
                }

                _nodes.Clear();
                _objects.Clear();
            }
        }

        private bool CanAdd(T obj)
        {
            if (_objectArea.GetTop(obj) > _area.Bottom || _objectArea.GetBottom(obj) < _area.Top || _objectArea.GetLeft(obj) > _area.Right || _objectArea.GetRight(obj) > _area.Left)
            {
                return false;
            }

            return true;
        }

        private bool IsOverlapping(QuadRect rect)
        {
            if (rect.Right < _area.Left || rect.Left > _area.Right)
                return false;

            if (rect.Top > _area.Bottom || rect.Bottom < _area.Top)
                return false;

            return true;
        }

        /// <summary>
        /// Quarter this node into four nodes and move existing objects into their respective nodes
        /// </summary>
        private void Quarter()
        {
            if (_currentDepth == _maxDepth)
                return;

            // since we are quartering, increase current depth
            _currentDepth++;

            // create the new quadrants
            _nodes[0] = new QuadNode<T>(_area.X, _area.Y, _area.HalfWidth, _area.HalfHeight, _objectArea, _maxDepth, _currentDepth, _maxChildrenPerNode); // TL
            _nodes[1] = new QuadNode<T>(_area.CenterX, _area.Y, _area.HalfWidth, _area.HalfHeight, _objectArea, _maxDepth, _currentDepth, _maxChildrenPerNode); // TR
            _nodes[2] = new QuadNode<T>(_area.X, _area.CenterY, _area.HalfWidth, _area.HalfHeight, _objectArea, _maxDepth, _currentDepth, _maxChildrenPerNode); // BL
            _nodes[3] = new QuadNode<T>(_area.CenterX, _area.CenterY, _area.HalfWidth, _area.HalfHeight, _objectArea, _maxDepth, _currentDepth, _maxChildrenPerNode); // BR

            // place objects into the new nodes
            foreach (var obj in _objects)
            {
                Insert(obj);
            }

            // clear objects from this now branch
            _objects.Clear();
        }
    }
}