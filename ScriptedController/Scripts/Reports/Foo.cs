using System.Web.Mvc;
public ActionResult Foo(string i) {
    return new JsonResult() {
        Data = "Foo - " + i,
        JsonRequestBehavior = JsonRequestBehavior.AllowGet
    };
}