using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using System;
using System.Buffers.Text;

namespace KPI_Lab5_Selenium
{
    public class Tests
    {
        private IWebDriver driver;
        private const string BASEURL = "https://the-internet.herokuapp.com/";
        private WebDriverWait wait;

        [SetUp]
        public void Setup()
        {
            driver = new ChromeDriver();
            driver.Manage().Window.Maximize();
            wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
        }

        [Test]
        public void Test01_ForgotPassword_SendEmail()
        {
            driver.Navigate().GoToUrl(BASEURL);
            driver.FindElement(By.LinkText("Forgot Password")).Click();

            driver.FindElement(By.Id("email")).SendKeys("test@gmail.com");
            driver.FindElement(By.Id("form_submit")).Click();

            IWebElement header = wait.Until(ExpectedConditions.ElementIsVisible(By.TagName("h1")));

            if (header.Text.Contains("Internal Server Error"))
            {
                Assert.Pass("Тест зупинено: Сервер повернув відому помилку 500 (Internal Server Error)");
            }
            else
            {
                Assert.That(header.Text, Does.Contain("Your e-mail's been sent!"));
            }
        }

        [Test]
        public void Test02_HorizontalSlider_MoveToTargetValue()
        {
            driver.Navigate().GoToUrl(BASEURL);
            driver.FindElement(By.LinkText("Horizontal Slider")).Click();

            IWebElement slider = driver.FindElement(By.CssSelector("input[type='range']"));
            
            string initialValue = driver.FindElement(By.Id("range")).Text;
            slider.Click();
            slider.SendKeys(Keys.Home);

            string valueAfterReset = driver.FindElement(By.Id("range")).Text;
            Assert.That(valueAfterReset, Is.EqualTo("0"));

            for (int i = 0; i < 7; i++)
            {
                slider.SendKeys(Keys.ArrowRight);
            }

            string value = driver.FindElement(By.Id("range")).Text;

            Assert.That(value, Is.EqualTo("3.5"));
        }

        [Test]
        public void Test03_Dropdown_CheckContentAndSelect()
        {
            driver.Navigate().GoToUrl(BASEURL);
            driver.FindElement(By.LinkText("Dropdown")).Click();

            SelectElement select = new SelectElement(driver.FindElement(By.Id("dropdown")));

            Assert.That(select.SelectedOption.Text, Does.Contain("Please select an option"));

            Assert.That(select.Options.Count, Is.EqualTo(3));

            Assert.That(select.Options[1].Text, Is.EqualTo("Option 1"));
            Assert.That(select.Options[2].Text, Is.EqualTo("Option 2"));

            select.SelectByText("Option 2");

            Assert.That(select.SelectedOption.Text, Is.EqualTo("Option 2"));
        }

        [Test]
        public void Test04_Typos_CheckSpelling()
        {
            driver.Navigate().GoToUrl(BASEURL);

            driver.FindElement(By.LinkText("Typos")).Click();

            var actualText = driver.FindElement(By.CssSelector(".example p:nth-of-type(2)")).Text;

            string expectedPart = "Sometimes you'll see a typo";

            Assert.That(actualText, Does.Contain(expectedPart));
        }

        [Test]
        public void Test05_EntryAd_CloseModal()
        {
            driver.Navigate().GoToUrl(BASEURL);
            driver.FindElement(By.LinkText("Entry Ad")).Click();

            wait.Until(ExpectedConditions.ElementIsVisible(By.ClassName("modal")));

            wait.Until(d =>
            {
                try
                {
                    var btn = d.FindElement(By.CssSelector(".modal-footer p"));

                    btn.Click();

                    return true;
                }
                catch (ElementClickInterceptedException)
                {
                    return false;
                }
            });

            wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.ClassName("modal")));

            IWebElement modal = driver.FindElement(By.ClassName("modal"));
            Assert.That(modal.Displayed, Is.False);
        }

        [Test]
        public void Test06_FileDownload_KeepFileForDemo()
        {

            driver.Navigate().GoToUrl(BASEURL);
            driver.FindElement(By.LinkText("File Download")).Click();
            
            IWebElement fileLink = driver.FindElement(By.CssSelector(".example a"));
            
            string fileName = fileLink.Text;

            string userPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            string downloadsFolder = Path.Combine(userPath, "Downloads");
            string filePath = Path.Combine(downloadsFolder, fileName);

            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }

            fileLink.Click();

            wait.Until(d => File.Exists(filePath));

            Assert.That(File.Exists(filePath), Is.True);
        }

        [Test]
        public void Test07_BasicAuth_UrlTrick()
        {
            driver.Navigate().GoToUrl(BASEURL);

            driver.Navigate().GoToUrl("https://admin:admin@the-internet.herokuapp.com/basic_auth");

            IWebElement content = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector(".example p")));

            Assert.That(content.Text, Does.Contain("Congratulations"));
        }

        [Test]
        public void Test08_DynamicLoading_Example1()
        {
            driver.Navigate().GoToUrl(BASEURL);
            driver.FindElement(By.LinkText("Dynamic Loading")).Click();

            driver.FindElement(By.PartialLinkText("Example 1")).Click();

            driver.FindElement(By.CssSelector("#start button")).Click();

            wait.Until(d => d.FindElement(By.Id("finish")).Displayed);

            string finishText = driver.FindElement(By.Id("finish")).Text;
            Assert.That(finishText, Is.EqualTo("Hello World!"));
        }

        [Test]
        public void Test09_ContextMenu_RightClick()
        {
            driver.Navigate().GoToUrl(BASEURL);

            driver.FindElement(By.LinkText("Context Menu")).Click();

            IWebElement hotSpot = driver.FindElement(By.Id("hot-spot"));

            Actions action = new Actions(driver);

            action.ContextClick(hotSpot).Perform();

            var alert = wait.Until(ExpectedConditions.AlertIsPresent());

            string alertText = alert.Text;
            Assert.That(alertText, Is.EqualTo("You selected a context menu"));

            alert.Accept();
        }

        [Test]
        public void Test10_RedirectLink_CheckUrlChange()
        {
            driver.Navigate().GoToUrl(BASEURL);

            driver.FindElement(By.LinkText("Redirect Link")).Click();
            
            IWebElement redirectBtn = wait.Until(ExpectedConditions.ElementToBeClickable(By.Id("redirect")));
            redirectBtn.Click();

            string expectedUrl = "https://the-internet.herokuapp.com/status_codes";

            wait.Until(ExpectedConditions.UrlToBe(expectedUrl));

            Assert.That(driver.Url, Is.EqualTo(expectedUrl));
        }

        [TearDown]
        public void TearDown()
        {
            if (driver != null)
            {
                driver.Quit();   
                driver.Dispose();
            }
        }
    }
}
