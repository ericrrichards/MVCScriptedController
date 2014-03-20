using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.IO;

namespace ScriptedController.Controllers {
    using System.Web.Routing;

    using CSScriptLibrary;

    public class ReportController : Controller {
        private static readonly object SyncRoot = new object();
        private volatile static Dictionary<string, MvcAction> _actions;
        internal delegate ActionResult MvcAction();

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
                    var path = Server.MapPath("~/Scripts/Reports");

                    foreach (var file in Directory.GetFiles(path)) {
                        try {
                            var action = CSScript.Evaluator.LoadDelegate<MvcAction>(System.IO.File.ReadAllText(file));
                            var actionName = Path.GetFileNameWithoutExtension(file);
                            if (!string.IsNullOrEmpty(actionName)) {
                                _actions[actionName] = action;
                            }
                        } catch (Exception ex) {
                            // log something out or do something clever
                        }
                    }
                }
            }
        }

        public ActionResult Foo1() {
            return new JsonResult() {
                Data = "Foo1",
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }
        
        protected override void HandleUnknownAction(string actionName) {
            if (_actions.ContainsKey(actionName)) {
                _actions[actionName]().ExecuteResult(ControllerContext);
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