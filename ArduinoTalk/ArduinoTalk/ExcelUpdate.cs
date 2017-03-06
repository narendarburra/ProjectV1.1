using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArduinoTalk
{
    class ExcelUpdate
    {

        public static void UpdateExcel(string sheetName, int row, int col, string data)
        {
            Microsoft.Office.Interop.Excel.Application oXL = null;
            Microsoft.Office.Interop.Excel._Workbook oWB = null;
            Microsoft.Office.Interop.Excel._Worksheet oSheet = null;

            try
            {
                oXL = new Microsoft.Office.Interop.Excel.Application();
                oWB = oXL.Workbooks.Open("C:\\Users\\nreddyburra\\Documents\\MyExcel.xlsx"); // C:\Users\nreddyburra\Documents\MyExcel.xlsx
                oSheet = String.IsNullOrEmpty(sheetName) ? (Microsoft.Office.Interop.Excel._Worksheet)oWB.ActiveSheet : (Microsoft.Office.Interop.Excel._Worksheet)oWB.Worksheets[sheetName];

                
                
                oSheet.Cells[row, 1] = DateTime.Now.ToString("hh:mm:ss");
                oSheet.Cells[row, col] = data;
                oWB.Save();

                //  MessageBox.Show("Done!");
            }
            catch (Exception ex)
            {
             //   MessageBox.Show(ex.ToString());
            }
            finally
            {
                if (oWB != null)
                    oWB.Close();
            }
        }

    }
}
