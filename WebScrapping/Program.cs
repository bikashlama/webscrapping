using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System.Threading;
using OpenQA.Selenium.Interactions;

namespace WebScrapping
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var driver = new ChromeDriver())
            {
                driver.Url = "login url of website";

                //load for page to load
                Thread.Sleep(3000);

                //click on login button
                driver.FindElementByLinkText("Log In").Click();

                //load for page to load
                Thread.Sleep(10000);

                //get the input fields and submit button
                var usernameField = driver.FindElementById("username");
                var passwordField = driver.FindElementById("password");
                var submitButton = driver.FindElement(By.XPath("//button[@type='submit'][text()='Next']"));

                //enter the credentials
                usernameField.SendKeys("username");
                passwordField.SendKeys("password");

                //click submit button
                submitButton.Click();

                //wait for another page to load after login
                Thread.Sleep(10000);

                //enter the url of the site where you want to navigate
                driver.Url = "url";

                //load for page to load fully
                Thread.Sleep(20000);

                //this would navigate to load all the items if the site uses lazy loading in site
                bool isLastItemFound = false;
                var scrollPoint = driver.FindElementByClassName("infinite-scroll");
                do
                {
                    Actions act = new Actions(driver);
                    act.MoveToElement(scrollPoint);
                    act.Perform();
                    Thread.Sleep(3000);
                    isLastItemFound = IsLastElementFound(driver, By.XPath("//a[@class='class-name']/div/h3[normalize-space()='Text']"));
                } while (isLastItemFound == false);

                //retrieve items by class class-name
                var items = driver.FindElements(By.XPath("//a[@class='class-name']"));

                //i am trying to get all the links which i would later use to navigate the page read the data, and get back to main page again
                IList<string> urls = new List<string>();
                for (int i = 0; i < items.Count; i++)
                {
                    //get the href value from item and add it to the string list of urls
                    urls.Add(items[i].GetAttribute("href"));
                }


                //iterate through all the urls
                for (int i = 0; i < urls.Count; i++)
                {
                    // save a reference to our original tab's window handle
                    var originalTabInstance = driver.CurrentWindowHandle;
                    // execute some JavaScript to open a new window
                    driver.ExecuteScript("window.open();");
                    // save a reference to our new tab's window handle, this would be the last entry in the WindowHandles collection
                    var newTabInstance = driver.WindowHandles[driver.WindowHandles.Count - 1];
                    // switch our WebDriver to the new tab's window handle
                    driver.SwitchTo().Window(newTabInstance);

                    // lets navigate to a web site in our new tab
                    driver.Navigate().GoToUrl(urls[i]);

                    //wait for the page to load
                    Thread.Sleep(15000);

                    /*
                     You can write codes here to access the element and read properties
                     here is just an example of getting name, phone and address
                     */
                    string name = GetElementText(driver, By.XPath("//h4[@data-name='']"));
                    string contactInfo = GetElementText(driver, By.XPath("//p[@data-contact-info='']"));
                    string email = GetElementText(driver, By.XPath("//p[@data-email='']/a"));
                    string mobilePhone = GetElementText(driver, By.XPath("//p[@data-mobile-phone='']"));

                    /*Then you can either write to database or file or do some other manipulations*/

                    // now lets close our new tab
                    driver.ExecuteScript("window.close();");
                    // and switch our WebDriver back to the original tab's window handle
                    driver.SwitchTo().Window(originalTabInstance);
                    // and have our WebDriver focus on the main document in the page to send commands to 
                    driver.SwitchTo().DefaultContent();
                }



            }
        }

        private static bool IsLastElementFound(ChromeDriver driver, By by)
        {
            try
            {
                driver.FindElement(by);
                return true;
            }
            catch (NoSuchElementException e)
            {
                return false;
            }
        }

        private static string GetElementText(ChromeDriver driver, By by)
        {
            try
            {
                return driver.FindElement(by).Text;
            }
            catch (NoSuchElementException e)
            {
                return string.Empty;
            }
        }
    }
}
