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

        [SetUp]
        public void Setup()
        {
            driver = new ChromeDriver();
            driver.Manage().Window.Maximize();
        }

        [Test]
        public void Test01_ForgotPassword_SendEmail()
        {
            driver.Navigate().GoToUrl(BASEURL);
            driver.FindElement(By.LinkText("Forgot Password")).Click();
            driver.FindElement(By.Id("email")).SendKeys("test@gmail.com");
            driver.FindElement(By.Id("form_submit")).Click();
           
            Thread.Sleep(2000);
            
            string pageText = driver.FindElement(By.TagName("h1")).Text;
            
            Assert.That(pageText, Is.EqualTo("Internal Server Error"));
        }

        [Test]
        public void Test02_HorizontalSlider_MoveToTargetValue()
        {
            driver.Navigate().GoToUrl(BASEURL);
            driver.FindElement(By.LinkText("Horizontal Slider")).Click();
            IWebElement slider = driver.FindElement(By.CssSelector("input[type='range']"));
            slider.Click();
            slider.SendKeys(Keys.Home);

            for (int i = 0; i < 7; i++)
            {
                slider.SendKeys(Keys.ArrowRight);
            }

            string value = driver.FindElement(By.Id("range")).Text;

            Assert.That(value, Is.EqualTo("3.5"), "Слайдер зупинився не на тому значенні!");
        }

        [Test]
        public void Test03_Dropdown_CheckContentAndSelect()
        {
            driver.Navigate().GoToUrl(BASEURL);
            driver.FindElement(By.LinkText("Dropdown")).Click();

            SelectElement select = new SelectElement(driver.FindElement(By.Id("dropdown")));

            Assert.That(select.Options.Count, Is.EqualTo(3));
            Thread.Sleep(2000);

            Assert.That(select.Options[1].Text, Is.EqualTo("Option 1"));
            Assert.That(select.Options[2].Text, Is.EqualTo("Option 2"));
            Thread.Sleep(2000);

            select.SelectByText("Option 2");

            Thread.Sleep(2000);

            Assert.That(select.SelectedOption.Text, Is.EqualTo("Option 2"));
        }

        [Test]
        public void Test04_Typos_CheckSpelling()
        {
            driver.Navigate().GoToUrl(BASEURL);

            driver.FindElement(By.LinkText("Typos")).Click();

            string actualText = driver.FindElement(By.XPath("//*[@id=\"content\"]/div/p[2]")).Text;

            Thread.Sleep(2000);

            string expectedText = "Sometimes you'll see a typo, other times you won't.";

            Assert.That(actualText, Is.EqualTo(expectedText), "У тексті знайдено орфографічну помилку!");
        }


        [Test]
        public void Test05_EntryAd_CloseModal_BestWay()
        {
            driver.Navigate().GoToUrl(BASEURL);
            driver.FindElement(By.LinkText("Entry Ad")).Click();

            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));

            IWebElement closeButton = wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath("//*[@id=\"modal\"]/div[2]/div[3]/p")));

            closeButton.Click();

            wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.ClassName("modal")));

            IWebElement modal = driver.FindElement(By.ClassName("modal"));
            Assert.That(modal.Displayed, Is.False, "Модальне вікно не закрилося!");
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

            Thread.Sleep(6000);

            Assert.That(File.Exists(filePath), Is.True, $"Файл {fileName} не завантажився!");
        }

        [Test]
        public void Test07_BasicAuth_UrlTrick()
        {
            driver.Navigate().GoToUrl(BASEURL);

            driver.Navigate().GoToUrl("https://admin:admin@the-internet.herokuapp.com/basic_auth");

            Thread.Sleep(1000);

            string pageText = driver.FindElement(By.CssSelector(".example p")).Text;

            Assert.That(pageText, Does.Contain("Congratulations"));
        }

        [Test]
        public void Test08_DynamicLoading_Example1()
        {
            driver.Navigate().GoToUrl(BASEURL);
            driver.FindElement(By.LinkText("Dynamic Loading")).Click();

            driver.FindElement(By.PartialLinkText("Example 1")).Click();

            driver.FindElement(By.XPath("//*[@id=\"start\"]/button")).Click();

            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));

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

            IAlert alert = driver.SwitchTo().Alert();
            Thread.Sleep(1000);
            string alertText = alert.Text;
            Assert.That(alertText, Is.EqualTo("You selected a context menu"), "Текст алерту неправильний!");

            alert.Accept();
        }

        [Test]
        public void Test10_RedirectLink_CheckUrlChange()
        {
            driver.Navigate().GoToUrl(BASEURL);

            driver.FindElement(By.LinkText("Redirect Link")).Click();
            Thread.Sleep(1000);
            driver.FindElement(By.Id("redirect")).Click();
            Thread.Sleep(1000);

            string expectedUrl = "https://the-internet.herokuapp.com/status_codes";

            Assert.That(driver.Url, Is.EqualTo(expectedUrl), "Редірект не перевів на очікувану сторінку!");
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
