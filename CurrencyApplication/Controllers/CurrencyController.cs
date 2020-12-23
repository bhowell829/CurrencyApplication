using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CurrencyApplication.Models;
using System.IO;
using Microsoft.AspNetCore.Hosting;

namespace CurrencyApplication.Controllers
{
    public class CurrencyController : Controller
    {
        private readonly IWebHostEnvironment _webHostEnvironment;

        public CurrencyController(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        public ActionResult Index()
        {
            //Creates lists
            CurrencyModel currencyModel = new CurrencyModel();

            //Checks if data is returned in GetInputData. If so, it loops through to add items to list
            if (GetInputData() != null)
            {
                currencyModel.InputList = GetInputData().ToList();
            }

            currencyModel.OutputList = new List<string>();

            //Returns model through view
            return View(currencyModel);
        }

        //Post index method that passes in CurrencyModel
        [HttpPost]
        public ActionResult Index(CurrencyModel currencyModel)
        {
            //Checks if data is returned in GetInputData and GetOutputData. If so, it loops through to add items to list
            if (GetInputData() != null)
            {
                currencyModel.InputList = GetInputData().ToList();
            }

            if (GetOutputData() != null)
            {
                currencyModel.OutputList = GetOutputData().ToList();
            }

            return View(currencyModel);
        }

        public List<string> GetInputData()
        {
            //Creates list and local variables, gets path from .txt file in App_Data and passes to StreamReader
            List<string> fileContent = new List<string>();
            string file = Path.Combine(_webHostEnvironment.WebRootPath, "files", "currency_sample.txt");
            if (file != null)
            {
                StreamReader sr = new StreamReader(file);
                string currentLine = string.Empty;

                //Loop that if currentLine is not null while reading lines, then will add currentline to fileContent list
                while ((currentLine = sr.ReadLine()) != null)
                {
                    fileContent.Add(currentLine);
                }

                //Close StreamReader
                sr.Close();

                //If else to see if fileContent list contains anything. If so, adds to array and returns array. If not, returns null.
                if (fileContent.Count != 0)
                {
                    //string[] lines = fileContent.ToArray();
                    return fileContent;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }

        public List<string> GetOutputData()
        {
            //Makes sure there is input data in .txt file
            if (GetInputData() != null)
            {
                //Creates stack
                Stack<string> inputStack = new Stack<string>(GetInputData().ToArray().Reverse());

                //Checks if stack is not empty, otherwise returns null
                if (inputStack.Count != 0)
                {
                    //Initializes new list and assigns element from stack to K and removes it from stack
                    List<string> result = new List<string>();
                    int dataSets = Int32.Parse(inputStack.Pop());

                    //Loop through as long as i is less than K
                    for (int i = 0; i < dataSets; i++)
                    {
                        //Initializes array, splits elements, and assigns next element from stack to denom_price and removes it from stack
                        string[] denom_price = inputStack.Pop().Split();

                        //Adds first index of denom_price array to int variable denom
                        int denom = Int32.Parse(denom_price[0]);
                        //Validation for range of denom and if outside, add -1 to result list for validation in View
                        if (denom < 2 || denom > 7)
                        {
                            result.Add("-1");
                            return result;
                        }

                        //Adds next index of denom_price array to int variable price
                        int price = Int32.Parse(denom_price[1]);
                        //Validation for range of price and if outside, add -1 to result list for validation in View
                        if (price < 2 || price > 10)
                        {
                            result.Add("-1");
                            return result;
                        }

                        //Create new list and assigns elements from stack to factors array and removes it from stack
                        var list = new List<int>();
                        string[] factors = inputStack.Pop().Split();

                        //D-1 loop then add elements from factors array to list
                        for (int j = 0; j < denom - 1; j++)
                        {
                            list.Add(Int32.Parse(factors[j]));
                        }

                        //Initializes minimum and maximum for 32 bit integer
                        int minimum = Int32.MaxValue;
                        int maximum = 0;

                        //Loops through prices as long as j is less than price
                        for (int j = 0; j < price; j++)
                        {
                            //Create new local variable total and assigns elements from stack to row array and removes it from stack
                            int total = 0;
                            string[] row = inputStack.Pop().Split();

                            //Loops through denom as long as k is less than price
                            for (int k = 0; k < denom; k++)
                            {
                                //Initializes num int with k index from q_row array then adds num to total
                                int num = Int32.Parse(row[k]);
                                total += num;

                                //Checks to see if k isn't equal to denom - 1 then multiplies index k from list array by total
                                if (k != denom - 1)
                                {
                                    total *= list[k];
                                }
                            }
                            //Assigns both minimum of two values and maximum of two values to variables
                            minimum = Math.Min(minimum, total);
                            maximum = Math.Max(maximum, total);
                        }
                        //Adds results of maximum minus minimum to result list
                        result.Add((maximum - minimum).ToString());
                    }
                    //Changes result list to array and returns it
                    return result;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }
    }
}
