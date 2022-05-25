
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace CG_1

{
    public struct Vertex
    {
        public double X { set; get; }
        public double Y { set; get; }
        public double Z { set; get; }
    
        public Vertex(double x, double y, double z) : this() {
            this.X = x;
            this.Y = y;
            this.Z = z;
        }
    }

    public struct Polygon
    {
        public int X { set; get; }
        public int Y { set; get; }
        public int Z { set; get; }
        public int W { set; get; }
    
        public Polygon(int x, int y, int z,int w) : this()
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
            this.W = w;
        }
    }
    
    public partial class Form1 : Form
    {
        private int _scale = 50;

        private readonly List<Polygon> _polygons;
        private List<Vertex> _vertices;
        private double[,] _verticiesMatrix;
        private readonly Pen _myPen;
        private Vertex _vertex;

        private double alpha = 0;

        private int totalDeltaX=0;
        private int totalDeltaY=0;
        


        public Form1()
        {
            InitializeComponent();
            _myPen = new Pen(Color.Black,3);
            
            var lines = File.ReadAllLines(@"cube3.obj");
            _vertices = new List<Vertex>();  
            _polygons = new List<Polygon>(); 
            foreach (var line in lines) {
                if (line.ToLower().StartsWith("v ")) {
                    var vx = line.Split(' ')
                        .Skip(1)
                        .Select(v => double.Parse(v.Replace('.', ',')))
                        .ToArray();
                    _vertices.Add(new Vertex(vx[0], vx[1], vx[2]));
                }
            
                else if (line.ToLower().StartsWith("f")) {
                    var vx = line.Split(' ')
                        .Skip(1)
                        .Select(int.Parse)
                        .ToArray();
                    _polygons.Add(new Polygon(vx[0]-1, vx[1]-1, vx[2]-1,vx[3]-1));
                }
            }

            _verticiesMatrix = ConvertListToMatrix(_vertices);

        }

        private void DrawPolygon(Polygon polygon,PaintEventArgs e)
        {
            
            e.Graphics.DrawLine(_myPen, (float)_vertices[polygon.X].X*_scale,(float)_vertices[polygon.X].Y*_scale,
                                     (float)_vertices[polygon.Y].X*_scale,(float)_vertices[polygon.Y].Y*_scale);
            
            e.Graphics.DrawLine(_myPen, (float)_vertices[polygon.Y].X*_scale,(float)_vertices[polygon.Y].Y*_scale,
                                     (float)_vertices[polygon.Z].X*_scale,(float)_vertices[polygon.Z].Y*_scale);
            
            e.Graphics.DrawLine(_myPen, (float)_vertices[polygon.Z].X*_scale,(float)_vertices[polygon.Z].Y*_scale,
                                        (float)_vertices[polygon.W].X*_scale,(float)_vertices[polygon.W].Y*_scale);
            
            e.Graphics.DrawLine(_myPen, (float)_vertices[polygon.W].X*_scale,(float)_vertices[polygon.W].Y*_scale,
                                     (float)_vertices[polygon.X].X*_scale,(float)_vertices[polygon.X].Y*_scale);
            
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.Clear(Color.Azure);
            foreach (var poly in _polygons)
            {
                DrawPolygon(poly,e);
            }

          
        }


        private double[,] Multiplication(double[,] matrixA, double[,] matrixB)
        {
            if (matrixA.GetLength(1) != matrixB.GetLength(0)) throw new Exception("Матрицы нельзя перемножить");
            var result = new double[matrixA.GetLength(0), matrixB.GetLength(1)];
            for (var i = 0; i < matrixA.GetLength(0); i++)
            {
                for (var j = 0; j < matrixB.GetLength(1); j++)
                {
                    for (var k = 0; k < matrixB.GetLength(0); k++)
                    {
                        result[i,j] += matrixA[i,k] * matrixB[k,j];
                    }
                }
            }
            return result;

        }

        private double[,] ConvertListToMatrix(List<Vertex> vertices)
        {
            var result = new double[vertices.Count,3];

            for (var i = 0; i < vertices.Count; i++)
            {
                result[i, 0] = vertices[i].X;
                result[i, 1] = vertices[i].Y;
                result[i, 2] = vertices[i].Z;
            }

            return result;
        }

        private List<Vertex> ConvertMatrixToList(double[,] matrix)
        {
            var result = new List<Vertex>();

            Vertex v;

            for (int i = 0; i < matrix.GetLength(0); i++)
            {
                v = new Vertex(matrix[i, 0], matrix[i, 1], matrix[i, 2]);
                result.Add(v);
            }
            
            
            return result;
        }

        private void trackBar1_ValueChanged(object sender, EventArgs e)
        {
            _scale = (trackBar1.Value+1)*50;
            pictureBox1.Refresh();
        }



        private void button_down_Click(object sender, EventArgs e)
        {
            _verticiesMatrix = ConvertListToMatrix(_vertices);
            var dx = 1;
            totalDeltaY += dx;

            for (int i = 0; i < _verticiesMatrix.GetLength(0); i++)
            {
                _verticiesMatrix[i, 1] += dx;
               
            }


            var result =ConvertMatrixToList( _verticiesMatrix);
            _vertices.Clear();
            _vertices = new List<Vertex>(result);
            pictureBox1.Refresh();
        }

        private void button_up_Click(object sender, EventArgs e)
        {
            
            _verticiesMatrix = ConvertListToMatrix(_vertices);
            var dx = 1;
            totalDeltaY -= dx;
            for (int i = 0; i < _verticiesMatrix.GetLength(0); i++)
            {
                _verticiesMatrix[i, 1] -= dx;
               
            }


            var result =ConvertMatrixToList( _verticiesMatrix);
            _vertices.Clear();
            _vertices = new List<Vertex>(result);
            pictureBox1.Refresh();
        }

        private void button_right_Click(object sender, EventArgs e)
        {
            _verticiesMatrix = ConvertListToMatrix(_vertices);
            var dy = 1;
            totalDeltaX += dy;

            for (int i = 0; i < _verticiesMatrix.GetLength(0); i++)
            {
                _verticiesMatrix[i, 0] += dy;
            }


            var result =ConvertMatrixToList( _verticiesMatrix);
            _vertices.Clear();
            _vertices = new List<Vertex>(result);
            pictureBox1.Refresh();
        }

        private void button_left_Click(object sender, EventArgs e)
        {
            _verticiesMatrix = ConvertListToMatrix(_vertices);
            var dy = 1;
            totalDeltaX -= dy;

            for (int i = 0; i < _verticiesMatrix.GetLength(0); i++)
            {
                _verticiesMatrix[i, 0] -= dy;
            }


            var result =ConvertMatrixToList( _verticiesMatrix);
            _vertices.Clear();
            _vertices = new List<Vertex>(result);
            pictureBox1.Refresh();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            _verticiesMatrix = ConvertListToMatrix(_vertices);
            
            for (int i = 0; i < _verticiesMatrix.GetLength(0); i++)
            {
                _verticiesMatrix[i, 0] -= totalDeltaX;
                _verticiesMatrix[i, 1] -= totalDeltaY;
            }

            alpha += (double)3*Math.PI/180;
            double[,] Mx =
            {
                {1, 0,0 },
                {0,Math.Cos(alpha), -Math.Sin(alpha) },
                {0, Math.Sin(alpha), Math.Cos(alpha) }
            };

            _verticiesMatrix= Multiplication(_verticiesMatrix, Mx);
            
            for (int i = 0; i < _verticiesMatrix.GetLength(0); i++)
            {
                _verticiesMatrix[i, 0] += totalDeltaX;
                _verticiesMatrix[i, 1] += totalDeltaY;
              
               
            }

            var result = ConvertMatrixToList(_verticiesMatrix);

           
           
            _vertices.Clear();
            _vertices = new List<Vertex>(result);
            pictureBox1.Refresh();
            
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            _verticiesMatrix = ConvertListToMatrix(_vertices);
            
            for (int i = 0; i < _verticiesMatrix.GetLength(0); i++)
            {
                _verticiesMatrix[i, 0] -= totalDeltaX;
                _verticiesMatrix[i, 1] -= totalDeltaY;
            }

            alpha -= (double)3*Math.PI/180;
            double[,] Mx =
            {
                {1, 0,0 },
                {0,Math.Cos(alpha), -Math.Sin(alpha) },
                {0, Math.Sin(alpha), Math.Cos(alpha) }
            };

            _verticiesMatrix= Multiplication(_verticiesMatrix, Mx);
            
            for (int i = 0; i < _verticiesMatrix.GetLength(0); i++)
            {
                _verticiesMatrix[i, 0] += totalDeltaX;
                _verticiesMatrix[i, 1] += totalDeltaY;
              
               
            }

            var result = ConvertMatrixToList(_verticiesMatrix);

           
           
            _vertices.Clear();
            _vertices = new List<Vertex>(result);
            pictureBox1.Refresh();
            
       
        }

        private void button3_Click(object sender, EventArgs e)
        {
            _verticiesMatrix = ConvertListToMatrix(_vertices);
            
            for (int i = 0; i < _verticiesMatrix.GetLength(0); i++)
            {
                _verticiesMatrix[i, 0] -= totalDeltaX;
                _verticiesMatrix[i, 1] -= totalDeltaY;
            }
            alpha -= (double)3*Math.PI/180;
            double[,] Mx =
            {
                {Math.Cos(alpha), 0,Math.Sin(alpha) },
                {0,1, 0 },
                {-Math.Sin(alpha), 0, Math.Cos(alpha) }
            };

            _verticiesMatrix= Multiplication(_verticiesMatrix, Mx);
            
            for (int i = 0; i < _verticiesMatrix.GetLength(0); i++)
            {
                _verticiesMatrix[i, 0] += totalDeltaX;
                _verticiesMatrix[i, 1] += totalDeltaY;
              
               
            }

            var result = ConvertMatrixToList(_verticiesMatrix);
            _vertices.Clear();
            _vertices = new List<Vertex>(result);
            pictureBox1.Refresh();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            _verticiesMatrix = ConvertListToMatrix(_vertices);
            
            for (int i = 0; i < _verticiesMatrix.GetLength(0); i++)
            {
                _verticiesMatrix[i, 0] -= totalDeltaX;
                _verticiesMatrix[i, 1] -= totalDeltaY;
            }
            alpha += (double)3*Math.PI/180;
            double[,] Mx =
            {
                {Math.Cos(alpha), 0,Math.Sin(alpha) },
                {0,1, 0 },
                {-Math.Sin(alpha), 0, Math.Cos(alpha) }
            };

            _verticiesMatrix= Multiplication(_verticiesMatrix, Mx);
            
            for (int i = 0; i < _verticiesMatrix.GetLength(0); i++)
            {
                _verticiesMatrix[i, 0] += totalDeltaX;
                _verticiesMatrix[i, 1] += totalDeltaY;
              
               
            }

            var result = ConvertMatrixToList(_verticiesMatrix);
            _vertices.Clear();
            _vertices = new List<Vertex>(result);
            pictureBox1.Refresh();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            _verticiesMatrix = ConvertListToMatrix(_vertices);
            
            for (int i = 0; i < _verticiesMatrix.GetLength(0); i++)
            {
                _verticiesMatrix[i, 0] -= totalDeltaX;
                _verticiesMatrix[i, 1] -= totalDeltaY;
            }
            alpha -= (double)3*Math.PI/180;
            double[,] Mx =
            {
                {Math.Cos(alpha), -Math.Sin(alpha),0 },
                {Math.Sin(alpha),Math.Cos(alpha), 0 },
                {0, 0, 1 }
            };

            _verticiesMatrix= Multiplication(_verticiesMatrix, Mx);
            
            for (int i = 0; i < _verticiesMatrix.GetLength(0); i++)
            {
                _verticiesMatrix[i, 0] += totalDeltaX;
                _verticiesMatrix[i, 1] += totalDeltaY;
              
               
            }

            var result = ConvertMatrixToList(_verticiesMatrix);
            
            _vertices.Clear();
            _vertices = new List<Vertex>(result);
            pictureBox1.Refresh();
        }

        private void button6_Click(object sender, EventArgs e)
        {
         
            _verticiesMatrix = ConvertListToMatrix(_vertices);
            
            for (int i = 0; i < _verticiesMatrix.GetLength(0); i++)
            {
                _verticiesMatrix[i, 0] -= totalDeltaX;
                _verticiesMatrix[i, 1] -= totalDeltaY;
            }
            alpha += (double)3*Math.PI/180;
            double[,] Mx =
            {
                {Math.Cos(alpha), -Math.Sin(alpha),0 },
                {Math.Sin(alpha),Math.Cos(alpha), 0 },
                {0, 0, 1 }
            };

            _verticiesMatrix= Multiplication(_verticiesMatrix, Mx);
            
            for (int i = 0; i < _verticiesMatrix.GetLength(0); i++)
            {
                _verticiesMatrix[i, 0] += totalDeltaX;
                _verticiesMatrix[i, 1] += totalDeltaY;
              
               
            }

            var result = ConvertMatrixToList(_verticiesMatrix);
            
            _vertices.Clear();
            _vertices = new List<Vertex>(result);
            pictureBox1.Refresh();
        }
    }
    
   
}
