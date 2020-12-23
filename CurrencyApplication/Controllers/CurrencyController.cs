using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CurrencyApplication.Models;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;

namespace CurrencyApplication.Controllers
{
    public class CurrencyController : Controller
    {
        private readonly ILogger<CurrencyController> _logger;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private CurrencyModel currencyModel;

        public CurrencyController(ILogger<CurrencyController> logger, IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
            _logger = logger;
        }

        public ActionResult Index()
        {
            currencyModel = new CurrencyModel();

            // Assign data to model or create new list for input and create new list for output on get
            if (GetInputData() != null)
            {
                currencyModel.InputList = GetInputData().ToList();
                _logger.LogInformation("Input data was successfully added");
            }
            else
            {
                currencyModel.InputList = new List<string>();
            }

            currencyModel.OutputList = new List<string>();

            return View(currencyModel);
        }

        [HttpPost]
        public ActionResult Index(CurrencyModel currencyModel)
        {
            if (ModelState.IsValid)
            {
                // Assign data to model or create new list for input and output on post
                if (GetInputData() != null)
                {
                    currencyModel.InputList = GetInputData().ToList();
                    _logger.LogInformation("Input data was successfully added");
                }
                else
                {
                    currencyModel.InputList = new List<string>();
                }

                if (GetOutputData() != null)
                {
                    currencyModel.OutputList = GetOutputData().ToList();
                    _logger.LogInformation("Output data was successfully added");
                }
                else
                {
                    currencyModel.OutputList = new List<string>();
                }

                return View(currencyModel);
            }
            return View();
        }

        public List<string> GetInputData()
        {
            // Get filepath of text file from root path
            List<string> inputList = new List<string>();
            string file = Path.Combine(_webHostEnvironment.WebRootPath, "files", "currencySample.txt");

            try
            {
                StreamReader streamReader = new StreamReader(file);
                string currentLine = string.Empty;

                // Continue through file as long as there is data
                while ((currentLine = streamReader.ReadLine()) != null)
                {
                    inputList.Add(currentLine);
                }

                streamReader.Close();

                _logger.LogInformation("File was found");
                return inputList;
            }
            catch (Exception e)
            {
                _logger.LogInformation("File was not found");
                throw e;
            }
        }

        public List<string> GetOutputData()
        {
            Stack<string> inputStack = new Stack<string>(GetInputData().ToArray().Reverse());
            List<string> outputList = new List<string>();
            int dataSets = Int32.Parse(inputStack.Pop());

            // Loop through dataSets
            for (int a = 0; a < dataSets; a++)
            {
                string[] denomPrice = inputStack.Pop().Split();

                int denom = Int32.Parse(denomPrice[0]);
                int price = Int32.Parse(denomPrice[1]);

                List<int> factorsList = new List<int>();
                string[] factors = inputStack.Pop().Split();

                // Validation for range of denom
                if (denom < 2 || denom > 7)
                {
                    outputList.Add("DenomOutOfScope");
                    return outputList;
                }

                // Validation for range of price
                if (price < 2 || price > 10)
                {
                    outputList.Add("PriceOutOfScope");
                    return outputList;
                }

                //Loop through denom - 1 then add elements from factors array to list
                for (int b = 0; b < denom - 1; b++)
                {
                    factorsList.Add(Int32.Parse(factors[b]));
                }

                int minimum = Int32.MaxValue;
                int maximum = 0;

                // Loop through prices
                for (int c = 0; c < price; c++)
                {
                    int totalQuantity = 0;
                    string[] quantity = inputStack.Pop().Split();

                    // Loop through denominations
                    for (int d = 0; d < denom; d++)
                    {
                        int numQuantity = Int32.Parse(quantity[d]);
                        totalQuantity += numQuantity;

                        // Check to see if d isn't equal to denom - 1 then multiplies index d from list array by total
                        if (d != denom - 1)
                        {
                            totalQuantity *= factorsList[d];
                        }
                    }

                    //Assign both minimum of two values and maximum of two values to variables
                    minimum = Math.Min(minimum, totalQuantity);
                    maximum = Math.Max(maximum, totalQuantity);
                }

                // Add results of maximum minus minimum to result list
                outputList.Add((maximum - minimum).ToString());
            }

            _logger.LogInformation("Output list created");
            return outputList;
        }
    }
}
