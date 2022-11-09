using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using prj40643133.Models;

namespace prj40643133.Controllers
{
    public class HomeController : Controller
    {
        dbBookStoreEntities db = new dbBookStoreEntities();
        // GET: Home
        public ActionResult Index()
        {
            //取得所有產品放入products
            ViewBag.Member = Session["Member"];
            ViewBag.Name = null;
            //若Session["Member"]為空，表示會員未登入
            if (Session["Member"] == null)
            {
                //指定Index.cshtml套用_Layout.cshtml，View使用products模型
                return View("Index", "_Layout");
            }
            //會員登入狀態
            //指定Index.cshtml套用_LayoutMember.cshtml，View使用products模型
            return View("Index", "_LayoutMember");
        }

        [HttpPost]
        public ActionResult Index(String BStoreName)
        {
            //取得所有產品放入products
            ViewBag.BStoreName = BStoreName;
            ViewBag.Member = Session["Member"];
            //若Session["Member"]為空，表示會員未登入
             if (Session["Member"] == null)
            {
            //指定Index.cshtml套用_Layout.cshtml，View使用products模型
                return View("BookStoreDetail", "_Layout");
            }
            //會員登入狀態
            //指定Index.cshtml套用_LayoutMember.cshtml，View使用products模型
            return View("BookStoreDetail", "_LayoutMember");
           // return RedirectToAction("BookStoreDetail", new { BStoreName = BStoreName });
        }

        [HttpPost]
        public ActionResult Recv(String favorite ,String address, String opentime, String phone , String image)
        {
            tFavorite myfavorite = new tFavorite();
            string fUserId = (Session["Member"] as tMember).fUserId;
            myfavorite.fUserId = fUserId;
            myfavorite.fBStoreName = favorite;
            myfavorite.fAddress = address;
            myfavorite.fOpentime = opentime;
            myfavorite.fPhone = phone;
            myfavorite.fImage = image;

            db.tFavorite.Add(myfavorite);
            db.SaveChanges();

            return RedirectToAction("Index");
        }

        //Get: Home/Login
        public ActionResult Login()
        {
            return View();
        }
        //Post: Home/Login
        [HttpPost]
        public ActionResult Login(string fUserId, string fPwd)
        {
            // 依帳密取得會員並指定給member
            var member = db.tMember
                .Where(m => m.fUserId == fUserId && m.fPwd == fPwd)
                .FirstOrDefault();
            //若member為null，表示會員未註冊
            if (member == null)
            {
                ViewBag.Message = "帳密錯誤，登入失敗";
                return View();
            }
            //使用Session變數記錄歡迎詞
            Session["WelCome"] = member.fName + " 歡迎光臨!";
            //使用Session變數記錄登入的會員物件
            Session["Member"] = member;
            //執行Home控制器的Index動作方法
            return RedirectToAction("Index");
        }

        //Get:Home/Register
        public ActionResult Register()
        {
            return View();
        }
        //Post:Home/Register
        [HttpPost]
        public ActionResult Register(tMember pMember)
        {
            //若模型沒有通過驗證則顯示目前的View
            if (ModelState.IsValid == false)
            {
                return View();
            }
            // 依帳號取得會員並指定給member
            var member = db.tMember
                .Where(m => m.fUserId == pMember.fUserId)
                .FirstOrDefault();
            //若member為null，表示會員未註冊
            if (member == null)
            {
                //將會員記錄新增到tMember資料表
                db.tMember.Add(pMember);
                db.SaveChanges();
                //執行Home控制器的Login動作方法
                return RedirectToAction("Login");
            }
            ViewBag.Message = "此帳號己有人使用，註冊失敗";
            return View();
        }

        //Get:Index/Logout
        public ActionResult Logout()
        {
            Session.Clear();  //清除Session變數資料
            return RedirectToAction("Index");
        }

        public ActionResult BookStoreDetail (string BStoreName)
        {
            ViewBag.BStoreName = BStoreName;
            ViewBag.Member = Session["Member"];
            if (Session["Member"] == null)
            {
                //指定Index.cshtml套用_Layout.cshtml，View使用products模型
                return View("BookStoreDetail", "_Layout");
            }
            //會員登入狀態
            //指定Index.cshtml套用_LayoutMember.cshtml，View使用products模型
            return View("BookStoreDetail", "_LayoutMember");
        }


        public ActionResult Myfavorite()
        {
            string fUserId = (Session["Member"] as tMember).fUserId;

            var myfavorites = db.tFavorite.Where(m => m.fUserId == fUserId)
                .OrderByDescending(m => m.fId).ToList();

            return View("Myfavorite", "_LayoutMember", myfavorites);
        }
        [HttpPost]
        public ActionResult Delete(int fId)
        {
            var Favorite = db.tFavorite.Where
                (m => m.fId == fId).FirstOrDefault();
            //刪除購物車狀態的產品
            db.tFavorite.Remove(Favorite);
            db.SaveChanges();

            return RedirectToAction("Myfavorite");
        }
    }
}