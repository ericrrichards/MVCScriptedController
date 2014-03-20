using System.Web.Mvc;
public ActionResult Fizz(string i) {
    return new JsonResult() {
        Data = "Fizz - " + i,
        JsonRequestBehavior = JsonRequestBehavior.AllowGet
    };
}