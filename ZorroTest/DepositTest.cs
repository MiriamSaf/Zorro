using Microsoft.VisualStudio.TestTools.UnitTesting;
using Zorro.WebApplication;
using Zorro.WebApplication.Controllers;
using Zorro.WebApplication.Models;
using Zorro.WebApplication.ViewModels;
using Zorro.WebApplication.Data;
using Zorro.Dal.Models;
using System;

namespace ZorroTest
{


    [TestClass]
    public class DepositTest
    {

        public String email1 = "adam@tester.com";
        [TestMethod]
        public void SignInUser()
        {
            ApplicationUser user1 = new ApplicationUser();
            user1.UserName = "Tester";
            user1.FirstName = "Adam";
            user1.Surname = "Test";
            user1.Email = "adam@tester.com";
            user1.PhoneNumber = "0411222333";
            user1.PasswordHash = "Tester123#";
            user1.Id = "123456";
            user1.CreditCardNumber = "1111222233334444";
            user1.CCExpiry = "11/23";

            Wallet walletTest = new Wallet();
            walletTest.ApplicationUserId = "123456";
            walletTest.ApplicationUser = user1;
            walletTest.Balance = 1000;





        }
    }
}