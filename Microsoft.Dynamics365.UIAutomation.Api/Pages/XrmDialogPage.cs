﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using Microsoft.Dynamics365.UIAutomation.Browser;
using OpenQA.Selenium;
using System;


namespace Microsoft.Dynamics365.UIAutomation.Api
{

    /// <summary>
    ///  Dialog page.
    ///  </summary>
    public class XrmDialogPage
       : XrmPage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="XrmDialogPage"/> class.
        /// </summary>
        /// <param name="browser">The browser.</param>
        public XrmDialogPage(InteractiveBrowser browser)
            : base(browser)
        {
            this.SwitchToDialog();
        }
        /// <summary>
        /// Enum for the Assign Dialog to determine which type of record you will be assigning to. 
        /// </summary>
        public enum AssignTo
        {
            Me,
            User,
            Team
        }

        public enum ReportRecords
        {
            AllRecords,
            SelectedRecords,
            AllRecordsOnPage
        }
        /// <summary>
        /// Closes the opportunity you are currently working on.
        /// </summary>
        /// <param name="revenue">The revenue you want to assign to the opportunity.</param>
        /// <param name="closeDate">The close date for the opportunity.</param>
        /// <param name="description">The description of the closing.</param>
        /// <param name="thinkTime">Used to simulate a wait time between human interactions. The Default is 2 seconds.</param>
        /// <example>xrmBrowser.Dialogs.CloseOpportunity(10000, DateTime.Now, "Testing the Close Opportunity API");</example>
        public BrowserCommandResult<bool> CloseOpportunity(double revenue, DateTime closeDate, string description, int thinkTime = Constants.DefaultThinkTime)
        {
            this.Browser.ThinkTime(thinkTime);
            return this.Execute(GetOptions("Close Opportunity"), driver =>
            {
                driver.WaitUntilAvailable(By.XPath(Elements.Xpath[Reference.Dialogs.Header]),
                                          new TimeSpan(0, 0, 10), 
                                          "The Close Opportunity dialog is not available.");

                SetValue(Elements.ElementId[Reference.Dialogs.CloseOpportunity.ActualRevenueId], revenue.ToString());
                SetValue(Elements.ElementId[Reference.Dialogs.CloseOpportunity.CloseDateId], closeDate);
                SetValue(Elements.ElementId[Reference.Dialogs.CloseOpportunity.DescriptionId], description);

                driver.ClickWhenAvailable(By.XPath(Elements.Xpath[Reference.Dialogs.CloseOpportunity.Ok]));

                return true;
            });
        }

        /// <summary>
        /// Assigns the record to a User or Team
        /// </summary>
        /// <param name="to">The User or Team you want to assign the record to</param>
        /// <param name="value">The value of the User or Team you want to find and select</param>
        /// <param name="thinkTime">Used to simulate a wait time between human interactions. The Default is 2 seconds.</param>
        /// <example>xrmBrowser.Dialogs.Assign(XrmDialogPage.AssignTo.Me, "");</example>
        public BrowserCommandResult<bool> Assign(AssignTo to, string value, int thinkTime = Constants.DefaultThinkTime)
        {
            this.Browser.ThinkTime(thinkTime);

            return this.Execute(GetOptions("Assign"), driver =>
            {
                driver.WaitUntilAvailable(By.XPath(Elements.Xpath[Reference.Dialogs.Header]),
                                          new TimeSpan(0, 0, 10),
                                          "The Assign dialog is not available.");

                switch (to)
                {
                    case AssignTo.Me:
                        driver.ClickWhenAvailable(By.XPath(Elements.Xpath[Reference.Dialogs.Assign.Ok]));
                       
                        break;

                    case AssignTo.User:
                        this.SetValue(new Lookup() { Name = Elements.ElementId[Reference.Dialogs.Assign.UserOrTeamLookupId], Value = value });
                        break;

                    case AssignTo.Team:
                        this.SetValue(new Lookup() { Name = Elements.ElementId[Reference.Dialogs.Assign.UserOrTeamLookupId]});
                        break;
                }

                return true;
            });
        }

        /// <summary>
        /// Deletes the selected record.
        /// </summary>
        /// <param name="thinkTime">Used to simulate a wait time between human interactions. The Default is 2 seconds.</param>
        /// <example>xrmBrowser.Dialogs.Delete();</example>
        public BrowserCommandResult<bool> Delete(int thinkTime = Constants.DefaultThinkTime)
        {
            this.Browser.ThinkTime(thinkTime);

            return this.Execute(GetOptions("Delete"), driver =>
            {
                driver.WaitUntilAvailable(By.XPath(Elements.Xpath[Reference.Dialogs.DeleteHeader]),
                                          new TimeSpan(0, 0, 10),
                                          "The Delete dialog is not available.");

                driver.ClickWhenAvailable(By.XPath(Elements.Xpath[Reference.Dialogs.Delete.Ok]));

                return true;
            });
        }

        /// <summary>
        /// Checks for Duplicate Detection Dialog. If duplicate detection is enable then you can confirm the save or cancel.
        /// </summary>
        /// <param name="save">If set to <c>true</c> Save the record otherwise it will cancel.</param>
        /// <param name="thinkTime">Used to simulate a wait time between human interactions. The Default is 2 seconds.</param>
        /// <example>xrmBrowser.Dialogs.DuplicateDetection(true);</example>
        public BrowserCommandResult<bool> DuplicateDetection(bool save, int thinkTime = Constants.DefaultThinkTime)
        {
            this.Browser.ThinkTime(thinkTime);

            return this.Execute(GetOptions("Duplicate Detection"), driver =>
            {
                driver.WaitUntilAvailable(By.XPath(Elements.Xpath[Reference.Dialogs.Header]), 
                                            new TimeSpan(0, 0, 5), 
                                            d => //If duplicate detection dialog shows up
                 {

                     if (save)
                         driver.ClickWhenAvailable(By.XPath(Elements.Xpath[Reference.Dialogs.DuplicateDetection.Save]));
                     else
                         driver.ClickWhenAvailable(By.XPath(Elements.Xpath[Reference.Dialogs.DuplicateDetection.Cancel]));
                 });

                return true;
            });
        }

        /// <summary>
        /// Run Work flow
        /// </summary>
        /// <param name="name">The name</param>
        /// <param name="thinkTime">Used to simulate a wait time between human interactions. The Default is 2 seconds.</param>
        /// <example>xrmBrowser.Dialogs.RunWorkflow("Account Set Phone Number");</example>
        public BrowserCommandResult<bool> RunWorkflow(string name, int thinkTime = Constants.DefaultThinkTime)
        {
            this.Browser.ThinkTime(thinkTime);

            return this.Execute(GetOptions("Run Workflow"), driver =>
            {
                driver.WaitUntilAvailable(By.XPath(Elements.Xpath[Reference.Dialogs.WorkflowHeader]),
                                          new TimeSpan(0, 0, 10),
                                          "The RunWorkflow dialog is not available.");

                var lookup = this.Browser.GetPage<XrmLookupPage>();
                Browser.Depth++;
                lookup.Search(name);
                lookup.SelectItem(name);
                lookup.Add();

                SwitchToDialog(1);
                driver.ClickWhenAvailable(By.XPath(Elements.Xpath[Reference.Dialogs.RunWorkflow.Confirm]));
                return true;
            });
        }


        /// <summary>
        /// Run Work flow
        /// </summary>
        /// <param name="name">The name</param>
        /// <param name="thinkTime">Used to simulate a wait time between human interactions. The Default is 2 seconds.</param>
        /// <example>xrmBrowser.Dialogs.RunWorkflow("Account Set Phone Number");</example>
        public BrowserCommandResult<bool> RunReport(ReportRecords records, int thinkTime = Constants.DefaultThinkTime)
        {
            this.Browser.ThinkTime(thinkTime);

            return this.Execute(GetOptions("Run Report"), driver =>
            {
                driver.WaitUntilAvailable(By.Name(Elements.Name[Reference.Dialogs.RunReport.Header]),
                                          new TimeSpan(0, 0, 10),
                                          "The Run Report dialog is not available.");
                switch (records)
                {
                    case ReportRecords.AllRecords:
                        driver.ClickWhenAvailable(By.XPath(Elements.Xpath[Reference.Dialogs.RunReport.Default]));

                        break;

                    case ReportRecords.SelectedRecords:
                        driver.ClickWhenAvailable(By.XPath(Elements.Xpath[Reference.Dialogs.RunReport.Selected]));
                        break;

                    case ReportRecords.AllRecordsOnPage:
                        driver.ClickWhenAvailable(By.XPath(Elements.Xpath[Reference.Dialogs.RunReport.View]));
                        break;
                }
                
                driver.ClickWhenAvailable(By.XPath(Elements.Xpath[Reference.Dialogs.RunReport.Confirm]));
                return true;
            });
        }
    }
}
