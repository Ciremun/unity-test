using System.Collections.Generic;
using UnityEngine;

namespace PathFind
{
/*
* Based on code and tutorial by Sebastian Lague (https://www.youtube.com/channel/UCmtyQOKKmrMVaKuRXz02jbQ).
*
* Author: Ronen Ness.
* Since: 2016.
*/
    public class Node
    {
        public bool walkable;
        public int gridX;
        public int gridY;
        public float price;

        public int gCost;
        public int hCost;
        public Node parent;
        public Node(float _price, int _gridX, int _gridY)
        {
            walkable = _price != 0.0f;
            price = _price;
            gridX = _gridX;
            gridY = _gridY;
        }
        public Node(bool _walkable, int _gridX, int _gridY)
        {
            walkable = _walkable;
            price = _walkable ? 1f : 0f;
            gridX = _gridX;
            gridY = _gridY;
        }
        public void Update(float _price, int _gridX, int _gridY) {
            walkable = _price != 0.0f;
            price = _price;
            gridX = _gridX;
            gridY = _gridY;
        }
        public void Update(bool _walkable, int _gridX, int _gridY) {
            walkable = _walkable;
            price = _walkable ? 1f : 0f;
            gridX = _gridX;
            gridY = _gridY;
        }
        public int fCost
        {
            get
            {
                return gCost + hCost;
            }
        }
    }
    public struct Point
    {
        public int x;
        public int y;
        public Point(int iX, int iY)
        {
            this.x = iX;
            this.y = iY;
        }
        public Point(Point b)
        {
            x = b.x;
            y = b.y;
        }
        public override int GetHashCode()
        {
            return x ^ y;
        }
        public override bool Equals(System.Object obj)
        {
            if (!(obj.GetType() == typeof(PathFind.Point)))
                return false;
            Point p = (Point)obj;
            if (ReferenceEquals(null, p))
            {
                return false;
            }
            return (x == p.x) && (y == p.y);
        }
        public bool Equals(Point p)
        {
            if (ReferenceEquals(null, p))
            {
                return false;
            }
            return (x == p.x) && (y == p.y);
        }
        public static bool operator ==(Point a, Point b)
        {
            if (System.Object.ReferenceEquals(a, b))
                return true;
            if (ReferenceEquals(null, a))
                return false;
            if (ReferenceEquals(null, b))
                return false;
            return a.x == b.x && a.y == b.y;
        }
        public static bool operator !=(Point a, Point b)
        {
            return !(a == b);
        }
        public Point Set(int iX, int iY)
        {
            this.x = iX;
            this.y = iY;
            return this;
        }
    }
    public class Grid
    {
        public Node[,] nodes;
        int gridSizeX, gridSizeY;

        public Grid(float[,] tiles_costs)
        {
            CreateNodes(tiles_costs.GetLength(0), tiles_costs.GetLength(1));
            for (int x = 0; x < gridSizeX; x++)
                for (int y = 0; y < gridSizeY; y++)
                    nodes[x, y] = new Node(tiles_costs[x, y], x, y);
        }

        public Grid(bool[,] walkable_tiles)
        {
            CreateNodes(walkable_tiles.GetLength(0), walkable_tiles.GetLength(1));
            for (int x = 0; x < gridSizeX; x++)
                for (int y = 0; y < gridSizeY; y++)
                    nodes[x, y] = new Node(walkable_tiles[x, y] ? 1.0f : 0.0f, x, y);
        }
        private void CreateNodes(int width, int height)
        {
            gridSizeX = width;
            gridSizeY = height;
            nodes = new Node[gridSizeX, gridSizeY];
        }
		public void UpdateGrid (float[,] tiles_costs)
        {
            if (nodes == null ||
                gridSizeX != tiles_costs.GetLength(0) ||
                gridSizeY != tiles_costs.GetLength(1))
            {
                CreateNodes(tiles_costs.GetLength(0), tiles_costs.GetLength(1));
            }
			for (int x = 0; x < gridSizeX; x++)
				for (int y = 0; y < gridSizeY; y++)
					nodes[x, y].Update(tiles_costs[x, y], x, y);
		}
		public void UpdateGrid (bool[,] walkable_tiles)
        {
            if (nodes == null ||
                gridSizeX != walkable_tiles.GetLength(0) ||
                gridSizeY != walkable_tiles.GetLength(1))
            {
                CreateNodes(walkable_tiles.GetLength(0), walkable_tiles.GetLength(1));
            }

			for (int x = 0; x < gridSizeX; x++)
				for (int y = 0; y < gridSizeY; y++)
					nodes[x, y].Update(walkable_tiles[x, y] ? 1.0f : 0.0f, x, y);
		}
        public System.Collections.IEnumerable GetNeighbours(Node node, Pathfinding.DistanceType distanceType)
        {
			int x = 0, y = 0;
            switch (distanceType)
            {
                case Pathfinding.DistanceType.Manhattan:
                    y = 0;
                    for (x = -1; x <= 1; ++x)
                    {
                        var neighbor = AddNodeNeighbour(x, y, node);
                        if (neighbor != null)
                            yield return neighbor;
                    }

                    x = 0;
                    for (y = -1; y <= 1; ++y)
                    {
                        var neighbor = AddNodeNeighbour(x, y, node);
                        if (neighbor != null)
                            yield return neighbor;
                    }
                    break;

                case Pathfinding.DistanceType.Euclidean:
                    for (x = -1; x <= 1; x++)
                    {
                        for (y = -1; y <= 1; y++)
                        {
                            var neighbor = AddNodeNeighbour(x, y, node);
                            if (neighbor != null)
                                yield return neighbor;
                        }
                    }
                    break;
            }
        }
        Node AddNodeNeighbour(int x, int y, Node node)
        {
            if (x == 0 && y == 0)
            {
                return null;
            }

            int checkX = node.gridX + x;
            int checkY = node.gridY + y;

            if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY)
            {
                return nodes[checkX, checkY];
            }

            return null;
        }
    }

    public class Pathfinding
    {
        public enum DistanceType
        {
            Euclidean,
            Manhattan
        }
        public static List<Point> FindPath(Grid grid, Point startPos, Point targetPos, DistanceType distance = DistanceType.Euclidean, bool ignorePrices = false)
        {
            List<Node> nodes_path = _ImpFindPath(grid, startPos, targetPos, distance, ignorePrices);
            List<Point> ret = new List<Point>();
            if (nodes_path != null)
                foreach (Node node in nodes_path)
                    ret.Add(new Point(node.gridX, node.gridY));
            return ret;
        }
        private static List<Node> _ImpFindPath(Grid grid, Point startPos, Point targetPos, DistanceType distance = DistanceType.Euclidean, bool ignorePrices = false)
        {
            Node startNode = grid.nodes[startPos.x, startPos.y];
            Node targetNode = grid.nodes[targetPos.x, targetPos.y];
            List<Node> openSet = new List<Node>();
            HashSet<Node> closedSet = new HashSet<Node>();
            openSet.Add(startNode);
            while (openSet.Count > 0)
            {
                Node currentNode = openSet[0];
                for (int i = 1; i < openSet.Count; i++)
                    if (openSet[i].fCost < currentNode.fCost || openSet[i].fCost == currentNode.fCost && openSet[i].hCost < currentNode.hCost)
                        currentNode = openSet[i];
                openSet.Remove(currentNode);
                closedSet.Add(currentNode);
                if (currentNode == targetNode)
                    return RetracePath(grid, startNode, targetNode);
                foreach (Node neighbour in grid.GetNeighbours(currentNode, distance))
                {
                    if (!neighbour.walkable || closedSet.Contains(neighbour))
                        continue;
                    int newMovementCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbour) * (ignorePrices ? 1 : (int)(10.0f * neighbour.price));
                    if (newMovementCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
                    {
                        neighbour.gCost = newMovementCostToNeighbour;
                        neighbour.hCost = GetDistance(neighbour, targetNode);
                        neighbour.parent = currentNode;
                        if (!openSet.Contains(neighbour))
                            openSet.Add(neighbour);
                    }
                }
            }
            return null;
        }
        private static List<Node> RetracePath(Grid grid, Node startNode, Node endNode)
        {
            List<Node> path = new List<Node>();
            Node currentNode = endNode;
            while (currentNode != startNode)
            {
                path.Add(currentNode);
                currentNode = currentNode.parent;
            }
            path.Reverse();
            return path;
        }
        private static int GetDistance(Node nodeA, Node nodeB)
        {
            int dstX = System.Math.Abs(nodeA.gridX - nodeB.gridX);
            int dstY = System.Math.Abs(nodeA.gridY - nodeB.gridY);
            return (dstX > dstY) ? 
                14 * dstY + 10 * (dstX - dstY) :
                14 * dstX + 10 * (dstY - dstX);
        }
    }
}
