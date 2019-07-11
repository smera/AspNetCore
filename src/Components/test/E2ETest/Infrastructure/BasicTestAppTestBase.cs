// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using BasicTestApp;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.E2ETest.Infrastructure.ServerFixtures;
using Microsoft.AspNetCore.E2ETesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace Microsoft.AspNetCore.Components.E2ETest.Infrastructure
{
    public class BasicTestAppTestBase : ServerTestBase<ToggleExecutionModeServerFixture<Program>>
    {
        public string ServerPathBase
            => "/subdir" + (_serverFixture.ExecutionMode == ExecutionMode.Server ? "#server" : "");

        public BasicTestAppTestBase(
            BrowserFixture browserFixture,
            ToggleExecutionModeServerFixture<Program> serverFixture,
            ITestOutputHelper output)
            : base(browserFixture, serverFixture, output)
        {
            serverFixture.PathBase = ServerPathBase;
        }

        protected IWebElement MountTestComponent<TComponent>() where TComponent : IComponent
        {
            var componentTypeName = typeof(TComponent).FullName;
            var testSelector = WaitUntilTestSelectorReady();
            testSelector.SelectByValue("none");
            testSelector.SelectByValue(componentTypeName);
            return Browser.FindElement(By.TagName("app"));
        }

        protected SelectElement WaitUntilTestSelectorReady()
        {
            var elemToFind = By.CssSelector("#test-selector > select");
            WaitUntilExists(elemToFind, timeoutSeconds: 30, throwOnError: true);
            return new SelectElement(Browser.FindElement(elemToFind));
        }

        protected IWebElement WaitUntilExists(By findBy, int timeoutSeconds = 10, bool throwOnError = false)
        {
            List<LogEntry> errors = null;
            IWebElement result = null;
            new WebDriverWait(Browser, TimeSpan.FromSeconds(timeoutSeconds)).Until(driver =>
            {
                if (throwOnError && Browser.Manage().Logs.AvailableLogTypes.Contains(LogType.Browser))
                {
                    // Fail-fast if any errors were logged to the console.
                    var log = Browser.Manage().Logs.GetLog(LogType.Browser);
                    if (log.Any(entry => entry.Level == LogLevel.Severe))
                    {
                        errors = log.Where(entry => entry.Level == LogLevel.Severe).ToList();
                        return true;
                    }
                }

                return (result = driver.FindElement(findBy)) != null;
            });

            if (errors != null)
            {
                var message =
                    $"Encountered errors while looking for '{findBy}'." + Environment.NewLine +
                    string.Join(Environment.NewLine, errors);
                throw new XunitException(message);
            }

            return result;
        }
    }
}
