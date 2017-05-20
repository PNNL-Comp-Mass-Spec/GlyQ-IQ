/* Written by Myanna Harris
* for the Department of Energy (PNNL, Richland, WA)
* Battelle Memorial Institute
* E-mail: Myanna.Harris@pnnl.gov
* Website: http://omics.pnl.gov/software
* -----------------------------------------------------
* 
 * Notice: This computer software was prepared by Battelle Memorial Institute,
* hereinafter the Contractor, under Contract No. DE-AC05-76RL0 1830 with the
* Department of Energy (DOE).  All rights in the computer software are reserved
* by DOE on behalf of the United States Government and the Contractor as
* provided in the Contract.
* 
 * NEITHER THE GOVERNMENT NOR THE CONTRACTOR MAKES ANY WARRANTY, EXPRESS OR
* IMPLIED, OR ASSUMES ANY LIABILITY FOR THE USE OF THIS SOFTWARE.
* 
 * This notice including this sentence must appear on any copies of this computer
* software.
* -----------------------------------------------------*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Shapes;
using System.Windows.Media;

namespace GridView4D
{
    class buildGrid
    {
        //adds row : grid = the Grid, num = the number of rows, size = height of each row
        public static void addRow(Grid grid, int num, int size)
        {
            for (int i = 1; i <= num; i++)
            {
                RowDefinition rowDefinition = new RowDefinition();

                if (size != 0)
                    rowDefinition.Height = new System.Windows.GridLength(size);
                grid.RowDefinitions.Add(rowDefinition);
            }
        }

        //adds column : grid = the Grid, num = the number of columns, size = height of each column
        public static void addCol(Grid grid, int num, int size)
        {
            for (int i = 1; i <= num; i++)
            {
                ColumnDefinition columnDefinition = new ColumnDefinition();

                if (size != 0)
                    columnDefinition.Width = new System.Windows.GridLength(size);
                grid.ColumnDefinitions.Add(columnDefinition);
            }
        }

        //adds Vertical bold grid line
        public static void addVert(Grid grid, int num, int mult)
        {
            for (int g = 0; g < num; g++)
            {
                //Add bold lines
                Line L = new Line();
                L.Style = (Style)Application.Current.Resources["verticalLineStyle"];
                L.Stretch = Stretch.Fill;
                Grid.SetColumn(L, g * mult);
                grid.Children.Add(L);
            }
        }

        //adds Horizontal bold grid line
        public static void addHoriz(Grid grid, int num, int mult)
        {
            for (int g = 0; g < num; g++)
            {
                //Add bold lines
                Line L = new Line();
                L.Style = (Style)Application.Current.Resources["horizontalLineStyle"];
                L.Stretch = Stretch.Fill;
                Grid.SetRow(L, g * mult);
                grid.Children.Add(L);
            }
        }
    }
}
