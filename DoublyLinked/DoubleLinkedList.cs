using System;
using System.Collections.Generic;
using System.IO;

namespace DoublyLinked
{
    public class DoubleLinkedList
    {
        private Node _head;
        public Node Head
        {
            get => _head;
            set => _head = value;
        }

        // A utility function to find the last node of linked list
        private Node LastNode(Node node)
        {
            while (node.Next != null)
                node = node.Next;
            return node;
        }

        /* Considers last element as pivot,
        places the pivot element at its
        correct position in a sorted array,
        and places all smaller (smaller than
        pivot) to left of pivot and all
        greater elements to right of pivot */
        private Node Partition(Node last, Node head)
        {
            // set pivot as h element
            string pivot = head.Data;

            // similar to i = l-1 for array implementation
            Node i = last.Prev;
            string temp;

            // Similar to "for (int j = l; j <= h- 1; j++)"
            for (Node j = last; j != head; j = j.Next)
            {
                if (j.Data.CompareTo(pivot) < 0)
                {
                    // Similar to i++ for array
                    i = (i == null) ? last : i.Next;
                    temp = i.Data;
                    i.Data = j.Data;
                    j.Data = temp;
                }
            }
            i = (i == null) ? last : i.Next; // Similar to i++
            temp = i.Data;
            i.Data = head.Data;
            head.Data = temp;
            return i;
        }

        /* A recursive implementation of
        quicksort for linked list */
        private void RecursiveQuickSort(Node last, Node head)
        {
            if (head != null && last != head && last != head.Next)
            {
                Node temp = Partition(last, head);
                RecursiveQuickSort(last, temp.Prev);
                RecursiveQuickSort(temp.Next, head);
            }
        }

        // The main function to sort a linked list.
        // It mainly calls _quickSort()
        public void QuickSort(Node node)
        {
            // Find last node
            Node head = LastNode(node);

            // Call the recursive QuickSort
            RecursiveQuickSort(node, head);
        }

        // A utility function to print contents of arr
        public void PrintList(Node head)
        {
            while (head != null)
            {
                Console.Write(head.Data + "\n");
                head = head.Next;
            }
        }

        /* Function to insert a node at the
        beginning of the Doubly Linked List */
        public void Push(string new_Data)
        {
            Node new_Node = new Node(new_Data); /* allocate node */

            // if head is null, head = new_Node
            if (_head == null)
            {
                _head = new_Node;
                return;
            }

            /* link the old list off the new node */
            new_Node.Next = _head;

            /* change prev of head node to new node */
            _head.Prev = new_Node;

            /* since we are adding at the
            beginning, prev is always NULL */

            /* move the head to point to the new node */
            _head = new_Node;
        }

        /* Driver code */

        public Dictionary<string, int> OccurancesOfElement(Node node)
        {
            Dictionary<string, int> visitedNode = new Dictionary<string, int>();
            while (node != null)
            {
                while (node != null)
                {
                    if (visitedNode.ContainsKey(node.Data))
                    {
                        int value = visitedNode[node.Data];
                        visitedNode[node.Data] = value + 1;
                    }
                    else
                    {
                        visitedNode.Add(node.Data, 1);
                    }
                    node = node.Next;
                }
            }
            
            using (var writer = new StreamWriter(Path.Combine(Path.GetTempPath(), "SaveFile.csv")))
            {
                foreach (var pair in visitedNode)
                {
                    writer.WriteLine("{0};{1};", pair.Key, pair.Value);
                }
            }
            return visitedNode;
        }
    }
}
