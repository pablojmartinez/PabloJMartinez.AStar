// Copyright (c) 2013-2017 Pablo J. Martínez. All rights reserved.
// Licensed under the MIT License.
// Part of the legacy PabloJMartinez.AStar implementation.

using System;
using System.Collections;
using UnityEngine;

namespace ComingLights
{
    /// <summary>
    /// Binary Heap (min-heap) for NodeRecords only, unless any other type is necessary, in which case look at Comparer.Compare.
    /// </summary>
    public class BinaryHeap
    {
        public NodeRecord[] Array;
        public int NextFreeSlot = 1;

        public BinaryHeap(int capacity)
        {
            // Starting index at 1 for an optimal implementation
            Array = new NodeRecord[capacity+1];
        }

        public void Insert(NodeRecord element)
        {
            if(NextFreeSlot == Array.Length)
            {
                System.Array.Resize<NodeRecord>(ref Array, Array.Length * 2);
            }
            Array[NextFreeSlot] = element;
            HeapifyUp(NextFreeSlot);
            NextFreeSlot++;
        }

        private void HeapifyUp(int slot)
        {
            NodeRecord element = Array[slot];
            int parent = slot/2;
            int child = slot;
            //for(int i = 1; i < NextFreeSlot; i++)
            //{
            while(element.EstimatedCost < Array[parent].EstimatedCost && parent > 0)
            {
                Array[child] = Array[parent];
                Array[parent] = element;
                child = parent;
                parent = parent/2;
            }
            //    else break;
            //}
        }

        public NodeRecord Extract()
        {
            NodeRecord root = Array[1];
            NextFreeSlot--; // Deletes last element making it free.
            NodeRecord lastElement = Array[NextFreeSlot];
            Array[1] = lastElement;
            HeapifyDown(1);
            /*if(lastElement.EstimatedCost > Array[2].EstimatedCost || lastElement.EstimatedCost > Array[3].EstimatedCost)
            {
                if(Array[2].EstimatedCost < Array[3].EstimatedCost)
                {
                    Array[1] = Array[2];
                    Array[2] = lastElement;
                }
                else
                {
                    Array[1] = Array[3];
                    Array[3] = lastElement;
                }
            }*/
            return root;
        }

        private int HeapifyDown(int slot)
        {
            NodeRecord element = Array[slot];
            int parent = slot;
            int leftChild = parent*2;
            int rightChild = leftChild+1;
            int finalSlot = -1;
            while((element.EstimatedCost > Array[leftChild].EstimatedCost || element.EstimatedCost > Array[rightChild].EstimatedCost) && (leftChild < NextFreeSlot && rightChild < NextFreeSlot))
            {
                if(Array[leftChild].EstimatedCost < Array[rightChild].EstimatedCost)
                {
                    Array[parent] = Array[leftChild];
                    Array[leftChild] = element;
                    finalSlot = leftChild;
                    parent = leftChild;
                }
                else
                {
                    Array[parent] = Array[rightChild];
                    Array[rightChild] = element;
                    finalSlot = rightChild;
                    parent = rightChild;
                }
                leftChild = parent*2;
                rightChild = leftChild+1;
            }
            return finalSlot;
        }

        public void Update(int elementToUpdate, NodeRecord newValue)
        {
            Array[elementToUpdate] = newValue;
            int parent = elementToUpdate/2;
            int leftChild = elementToUpdate*2;
            int rightChild = leftChild+1;

            if(parent > 1)
            {
                if(Array[elementToUpdate].EstimatedCost < Array[parent].EstimatedCost)
                {
                    HeapifyUp(elementToUpdate);
                }
            }

            if(leftChild < NextFreeSlot)
            {
                if(Array[elementToUpdate].EstimatedCost > Array[leftChild].EstimatedCost)
                {
                    HeapifyDown(elementToUpdate);
                }
            }

            if(rightChild < NextFreeSlot)
            {
                if(Array[elementToUpdate].EstimatedCost > Array[rightChild].EstimatedCost)
                {
                    HeapifyDown(elementToUpdate);
                }
            }
        }

        public void Clear()
        {
            //System.Array.Clear(Array, 0, NextFreeSlot);
            NextFreeSlot = 1;
        }
    }
}