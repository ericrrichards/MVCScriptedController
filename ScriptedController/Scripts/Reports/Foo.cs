using System.Web.Mvc;
public ActionResult Foo() {
    return new JsonResult() {
        Data = "Foo",
        JsonRequestBehavior = JsonRequestBehavior.AllowGet
    };
}