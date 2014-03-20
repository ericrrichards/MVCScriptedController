using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.IO;

namespace ScriptedController.Controllers {
    using System.Web.Routing;

    using CSScriptLibrary;

    public class ReportController : Controller {
        private const string ScriptsDirectory = "~/Scripts/Reports";

        private static readonly object SyncRoot = new object();
        private volatile static Dictionary<string, MvcAction> _actions;
        private delegate ActionResult MvcAction(string i);

        protected override void Initialize(RequestContext requestContext) {
            base.Initialize(requestContext);
            EnsureActionsLoaded();
        }

        private void EnsureActionsLoaded() {
            if (_actions != null) {
                return;
            }
            lock (SyncRoot) {
                if (_actions == null) {
                    _actions = new Dictionary<string, MvcAction>();
                    var path = Server.MapPath(ScriptsDirectory);

                    foreach (var file in Directory.GetFiles(path)) {
                        try {
                            var action = CSScript.Evaluator.LoadDelegate<MvcAction>(System.IO.File.ReadAllText(file));
                            var actionName = Path.GetFileNameWithoutExtension(file);
                            if (!string.IsNullOrEmpty(actionName)) {
                                _actions[actionName] = action;
                            }
                        } catch (Exception ex) {
                            Console.WriteLine(ex.Message);
                        }
                    }
                }
            }
        }
        
        protected override void HandleUnknownAction(string actionName) {
            if (_actions.ContainsKey(actionName)) {
                _actions[actionName](HttpContext.Request.Params["i"]).ExecuteResult(ControllerContext);
            } else {
                try {
                    base.HandleUnknownAction(actionName);
                } catch (Exception ex) {
                    HttpContext.Response.StatusCode = 404;
                }
            }
        }
    }
}