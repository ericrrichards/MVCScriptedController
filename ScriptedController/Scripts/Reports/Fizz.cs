using System.Web.Mvc;
public ActionResult Fizz() {
    return new JsonResult() {
        Data = "Fizz",
        JsonRequestBehavior = JsonRequestBehavior.AllowGet
    };
}