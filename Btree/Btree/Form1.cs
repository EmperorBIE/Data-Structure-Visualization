﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Btree {
    public partial class Form1 : Form {
        /* List bug :
         * bug posisi ketika parent punya 2 child (total 3 node)
         * bug search kotak/string
         * bug search line
         * posisi gambar mulai dari kiri
         * 
         */
        Btree bt;
        bool onSearch;
        int ordoSize, insertValue, deleteValue, searchValue;

        int width = 854, height = 480;

        public Form1() {
            InitializeComponent();
            onSearch = false;
        }

        private void BtnOrdo_Click(object sender, EventArgs e) {
            SetOrdo();
        }
        private void TbOrdo_KeyUp(object sender, KeyEventArgs e) {
            if (e.KeyCode == Keys.Enter)
                SetOrdo();
        }
        private void SetOrdo() {
            ordoSize = int.Parse(tbOrdo.Text);
            bt = new Btree(ordoSize);
            tbOrdo.Text = "";
            panel1.Refresh();

            tbInsert.Enabled = true;
            btnInsert.Enabled = true;
            tbSearch.Enabled = true;
            btnSearch.Enabled = true;
            tbDelete.Enabled = true;
            btnDelete.Enabled = true;

            btnOrdo.Text = "Reset Tree";
        }

        private void BtnInsert_Click(object sender, EventArgs e) {
            InsertKey();
        }
        private void TbInsert_KeyUp(object sender, KeyEventArgs e) {
            if (e.KeyCode == Keys.Enter)
                InsertKey();
        }
        private void InsertKey() {
            insertValue = int.Parse(tbInsert.Text);
            bt.insert(ref bt.root, insertValue);
            Console.WriteLine("Insert " + tbInsert.Text);
            tbInsert.Text = "";
            panel1.Refresh();
        }

        private void BtnSearch_Click(object sender, EventArgs e) {
            SearchKey();
        }
        private void TbSearch_KeyUp(object sender, KeyEventArgs e) {
            if (e.KeyCode == Keys.Enter)
                SearchKey();
        }
        private void SearchKey() {
            searchValue = int.Parse(tbSearch.Text);
            if (bt.find(bt.root, searchValue) == null) {

            } else {
                onSearch = true;
                panel1.Refresh();
                onSearch = false;
            }
            tbSearch.Text = "";
        }

        private void BtnDelete_Click(object sender, EventArgs e) {
            DeleteKey();
        }
        private void TbDelete_KeyUp(object sender, KeyEventArgs e) {
            if (e.KeyCode == Keys.Enter)
                DeleteKey();
        }
        private void DeleteKey() {
            int deleteValue = int.Parse(tbDelete.Text);
            bt.delete(ref bt.root, deleteValue);
            tbDelete.Text = "";
            panel1.Refresh();
        }

        private void Panel1_Paint(object sender, PaintEventArgs e) {
            if (bt != null) {
                // normal
                Pen p = new Pen(Color.Black);
                Brush b = new SolidBrush(Color.Black);
                Font f = new Font("Courier New", 10, FontStyle.Regular);

                // to be drawn
                List<Rectangle> nodeRectangle = new List<Rectangle>();
                List<LinePosition> lines = new List<LinePosition>();

                // for search
                Pen ps = new Pen(Color.Red);
                Brush bs = new SolidBrush(Color.Red);
                Font fs = new Font("Courier New", 10, FontStyle.Bold);

                const int RECTANGLE_SIZE = 30, RECTANGLE_Y_DISTANCE = 20;// RECTANGLE_SIZE also used as RECTANGLE_X_DISTANCE

                List<FakeBNode> fakeBNodes = bt.getFakeBNodes();

                int lastDepth = 0;
                int[] keysCount = new int[bt.maxDepth + 1], nodeCount = new int[bt.maxDepth + 1], childCount = new int[bt.maxDepth + 1];
                // i,j,... = fakeBNodes
                // a,b,... = fakeBNodes.keys
                // x,y,... = fakeBNodes.child
                for (int i = 0; i < fakeBNodes.Count; i++) {
                    // step 1. to determine key position
                    if (fakeBNodes[i].depth != lastDepth) {
                        lastDepth = fakeBNodes[i].depth;
                        nodeCount[lastDepth] = 0;
                        childCount[lastDepth] = fakeBNodes[i].childCount;
                        keysCount[lastDepth] = fakeBNodes[i].keysCount;
                    } else {
                        if (i != 0)
                            nodeCount[lastDepth]++;
                        childCount[lastDepth] += fakeBNodes[i].childCount;
                        keysCount[lastDepth] += fakeBNodes[i].keysCount;
                    }

                    for (int a = 0; a < fakeBNodes[i].keysCount; a++) {
                        // step 2. set key position & draw key + rectangle
                        int x = (keysCount[lastDepth] - fakeBNodes[i].keysCount + a) * RECTANGLE_SIZE + nodeCount[lastDepth] * RECTANGLE_SIZE;
                        int y = lastDepth * (RECTANGLE_SIZE + RECTANGLE_Y_DISTANCE);
                        bool isCorrelated = false; // only used when on searcg

                        if (onSearch) {
                            // jika value sama
                            if (fakeBNodes[i].keys[a] == searchValue) {
                                isCorrelated = true;
                            }
                            // parent && value diantara
                            if (a < fakeBNodes[i].keysCount - 1 && searchValue > fakeBNodes[i].keys[a] && searchValue < fakeBNodes[i].keys[a + 1]) {
                                isCorrelated = true;
                            }
                            if (a > 0 && searchValue > fakeBNodes[i].keys[a - 1] && searchValue < fakeBNodes[i].keys[a]) {
                                isCorrelated = true;
                            }
                            // parent && value lebih besar - buggy
                            if(fakeBNodes[i].depth < bt.maxDepth && a == fakeBNodes[i].keysCount - 1 && searchValue > fakeBNodes[i].keys[a]){
                                isCorrelated = true;
                            }
                            // parent && value lebih kecil - buggy
                            if (fakeBNodes[i].depth < bt.maxDepth && a == 0 && searchValue < fakeBNodes[i].keys[a]) {
                                isCorrelated = true;
                            }
                        }

                        if (isCorrelated) {
                            nodeRectangle.Add(new Rectangle(x, y, RECTANGLE_SIZE, RECTANGLE_SIZE));
                            e.Graphics.DrawRectangle(ps, x, y, RECTANGLE_SIZE, RECTANGLE_SIZE);
                            e.Graphics.DrawString(fakeBNodes[i].keys[a].ToString(), fs, b, x, y);
                        } else {
                            e.Graphics.DrawRectangle(p, x, y, RECTANGLE_SIZE, RECTANGLE_SIZE);
                            e.Graphics.DrawString(fakeBNodes[i].keys[a].ToString(), f, b, x, y);
                        }
                    }
                }

                // step 3. prepare line
                int currentNodeIndex = -1, currentChildIndex = 0, currentChildCount = 0;
                int childNodeIndex = 0, childKeyCount = 0;
                lastDepth = 0;
                for (int i = 0; i < fakeBNodes.Count; i++) {
                    if (fakeBNodes[i].depth != lastDepth) {
                        currentNodeIndex = 0;
                        currentChildIndex = 0;
                        currentChildCount = fakeBNodes[i].childCount;
                        lastDepth = fakeBNodes[i].depth;
                    } else {
                        currentNodeIndex++;
                        currentChildCount += fakeBNodes[i].childCount;
                    }

                    if (fakeBNodes[i].depth < bt.maxDepth) {
                        for (int x = 0; x < fakeBNodes[i].childCount; x++) {
                            childNodeIndex = 0;
                            childKeyCount = 0;
                            for (int j = i + 1; j < fakeBNodes.Count; j++) {
                                if (fakeBNodes[i].depth + 1 == fakeBNodes[j].depth) {
                                    childKeyCount += fakeBNodes[j].keysCount;
                                    if (childNodeIndex == currentChildIndex) {
                                        Console.WriteLine("================================");
                                        Console.WriteLine("Node parent : " + i);
                                        Console.WriteLine("currentChildCount : " + currentChildCount);
                                        Console.WriteLine("fakeBNodes[i].childCount : " + fakeBNodes[i].childCount);
                                        Console.WriteLine("x : " + x);
                                        Console.WriteLine("currentNodeIndex : " + currentNodeIndex);

                                        int x1 = (currentNodeIndex + x) * RECTANGLE_SIZE + currentNodeIndex * RECTANGLE_SIZE; // bug
                                        int y1 = lastDepth * (RECTANGLE_SIZE + RECTANGLE_Y_DISTANCE) + RECTANGLE_SIZE;
                                        int x2 = childNodeIndex * RECTANGLE_SIZE + (childKeyCount - fakeBNodes[j].keysCount) * RECTANGLE_SIZE;
                                        int y2 = y1 + RECTANGLE_Y_DISTANCE;
                                        lines.Add(new LinePosition(x1, y1, x2, y2));
                                    }
                                    childNodeIndex++;
                                }
                            }
                            currentChildIndex++;
                        }
                    }
                }

                // step 4. draw line
                for (int m = 0; m < lines.Count; m++) {
                    // color buggy
                    bool isCorrelated = false;
                    for (int i = 0; i < nodeRectangle.Count; i++) {
                        Rectangle tempIn = new Rectangle(lines[m].x1, lines[m].y1, lines[m].x2, lines[m].y2);
                        if (nodeRectangle[i].IntersectsWith(tempIn)) {
                            isCorrelated = true;
                            break;
                        }
                    }

                    if (isCorrelated) {
                        e.Graphics.DrawLine(ps, lines[m].x1, lines[m].y1, lines[m].x2, lines[m].y2);
                    } else {
                        e.Graphics.DrawLine(p, lines[m].x1, lines[m].y1, lines[m].x2, lines[m].y2);
                    }
                }

                //Console.WriteLine("Inorder!");
                //bt.inorder(ref bt.root);
            }
        }
    }
}
