﻿namespace DotSee.Discipline.NodeProtect
{
    /// <summary>
    /// Holds rules for restricting node publishing.
    /// </summary>
    public class Rule
    {
        /// <summary>
        /// Doctype alias that will not be allowed to be deleted
        /// </summary>
        public string DocTypeAlias { get; set; }
        /// <summary>
        /// Document GUIDs that will not be allowed to be deleted
        /// </summary>
        public string DocumentGuids { get; set; }
        /// <summary>
        /// If true, a warning message will be displayed when a node is published and a rule is in effect, if the limit has not been reached
        /// </summary>
        public string CustomMessage { get; set; }
        /// <summary>
        /// A custom "limit reached" message. This overrides the standard message.
        /// </summary>
        public string CustomMessageCategory { get; set; }
        /// <summary>
        /// Set this to true to make the rule be ignored if the user performing the action is an admin
        /// </summary>
        public bool IgnoreForAdmins { get; set; }

        /// <summary>
        /// Holds the data for a node restriction rule.
        /// </summary>
        /// <param name="docTypeAlias">Doctype alias that will not be allowed to be deleted</param>
        /// <param name="documentGuids">Document GUIDs that will not be allowed to be deleted</param>
        /// <param name="fromProperty">Indicates whether the rule has been created on the fly based on the document's special property (true) or is a rule that comes from the config file (false)</param>
        /// <param name="ignoreForAdmins">Set this to true to make the rule be ignored if the user performing the action is an admin</param>
        /// <param name="customMessage">A custom "limit reached" message. This overrides the standard message.</param>
        /// <param name="customMessageCategory">Custom category for the "limit reached" message. This overrides the standard category literal.</param>
        public Rule(
            string docTypeAlias,
            string documentGuids,
            bool ignoreForAdmins = false,
            string customMessage = "",
            string customMessageCategory = "")
        {
            DocTypeAlias = docTypeAlias;
            DocumentGuids = documentGuids;
            IgnoreForAdmins = ignoreForAdmins;
            CustomMessage = customMessage;
            CustomMessageCategory = customMessageCategory;
        }
    }
}
