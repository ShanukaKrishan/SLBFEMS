using System;
using Microsoft.Extensions.Logging;
using SLBFEMS.Enums;
using SLBFEMS.Interfaces;
using SLBFEMS.Models;
using SLBFEMS.ViewModels.Authentication;

namespace SLBFEMS.Services
{
    public class AuthService : IAuthService
    {
        private readonly IEmailService _emailService;
        private readonly ILogger<AuthService> _logger;
        public AuthService(IEmailService emailService, ILogger<AuthService> logger)
        {
            _emailService = emailService;
            _logger = logger;
        }

        public NICInfoViewModel GetNICInfo(string nic)
        {
            try
            {
                int year = 0, days = 0, bmonth = 0, bdate = 0;
                int[] month = new int[12] { 31, 29, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31 };
                Genders gender;

                if (nic.Length == 10)
                {
                    year = 1900 + Int32.Parse(nic.Substring(0, 2));
                    days = Int32.Parse(nic.Substring(2, 3));
                }
                else
                {
                    year = Int32.Parse(nic.Substring(0, 4));
                    days = Int32.Parse(nic.Substring(4, 3));
                }

                if (days > 500)
                {
                    gender = Genders.Female;
                    days = days - 500;
                }
                else
                {
                    gender = Genders.Male;
                }

                for (int i = 0; i < month.Length; i++)
                {
                    if (days < month[i])
                    {
                        bmonth = i + 1;
                        bdate = days;
                        break;
                    }
                    else
                    {
                        days = days - month[i];
                    }
                }

                return new NICInfoViewModel { Gender = gender, Birthday = DateTime.Parse(year + "/" + bmonth + "/" + bdate) };
            }
            catch (Exception ex)
            {
                _logger.LogError("Error occured in nic processing", ex.Message);
                throw;
            }
        }

        public void SendEmail(string type, string inputEmail = null, ApplicationUserModel user = null, string token = null)
        {
            string subject = "";
            string html = "";
            string verifyUrl;
            string email = user != null ? user.Email : inputEmail;

            try
            {
                switch (type)
                {
                    case "verify":
                        verifyUrl = $"https://predictly.z13.web.core.windows.net/auth/confirm-email?userid={user.Id}&token={token}";
                        subject = "Sign-up Verification Vaccination Management System - Verify Email";
                        html =
                        $@" <center><img
                                style=""width: 40%""
                                src='https://docs.google.com/uc?id=1kIvq5gRqlUM_y-Y-7KpQw3oGtuX7Im0A'
                                alt=''
                                />
                            <h2>
                                Please click the below button to <br /> verify your email
                            </h2>
                            <br />
                                <a
                                style=""
                                    border-radius: 5px;
                                    color: white;
                                    background-color: rgb(4, 128, 201);
                                    padding: 15px;
                                    border: none;
                                    letter-spacing: 0.1rem;
                                    text-transform: uppercase;
                                    text-decoration: none;
                                ""
                                href=""{verifyUrl}""
                                >
                                Verify email
                                </a></center>";
                        break;
                    case "resetPass":
                        verifyUrl = $"https://predictly.z13.web.core.windows.net/auth/reset-password?userid={user.Id}&token={token}";
                        subject = "Vaccination Management System - Reset password";
                        html = $@" <center>
                                <img
                                style=""width: 40%""
                                src='https://docs.google.com/uc?id=12MmOUkndXs65qf7kd6FCzV4iZGKPF16s'
                                alt=''
                                />
                            <h2 style=""
                                    color: black;
                                "">
                                Please click the below button to <br />
                                reset your password
                            </h2>
                            <br />
                                <a
                                style=""
                                    border-radius: 5px;
                                    color: white;
                                    background-color: rgb(255, 115, 0);
                                    padding: 15px;
                                    border: none;
                                    letter-spacing: 0.1rem;
                                    text-transform: uppercase;
                                    text-decoration: none;
                                ""
                                href=""{verifyUrl}""
                                >
                                Reset Password
                                </a>
                            </center>";
                        break;
                    case "verified":
                        subject = "Sign-up Verification Vaccination Management System";
                        html =
                        $@" <center><img
                                style=""width: 40%""
                                src='https://docs.google.com/uc?id=1LqFsaoDVUdXQMUMoEZ8MkNTjDiYQp1FZ'
                                alt=''
                                />
                            <h2 style=""
                                    color: black;
                                "">
                                Your email verification is successfull
                            </h2>
                            <br />
                                <a
                                style=""
                                    border-radius: 5px;
                                    color: white;
                                    background-color: rgb(37, 199, 50);
                                    padding: 15px;
                                    border: none;
                                    letter-spacing: 0.1rem;
                                    text-transform: uppercase;
                                    text-decoration: none;
                                ""
                                href=""https://predictly.z13.web.core.windows.net/auth/login""
                                >
                                Continue to Login
                                </a></center>";
                        break;
                    case "resetted":
                        subject = "Password Reset Successfull";
                        html =
                        $@" <center>
                                <img
                                style=""width: 40%""
                                src='https://docs.google.com/uc?id=1tQNONuwfg5phj1teyBbG7W02lpQ6nPBi'
                                alt=''
                                />
                            <h2>
                                Password Resetted Successfully!
                            </h2>
                            <br />
                                <a
                                style=""
                                    border-radius: 5px;
                                    color: white;
                                    background-color: rgb(143, 179, 46);
                                    padding: 15px;
                                    border: none;
                                    letter-spacing: 0.1rem;
                                    text-transform: uppercase;
                                    text-decoration: none;
                                ""
                                href=""https://predictly.z13.web.core.windows.net/auth/login""
                                >
                                Continue to Login
                                </a>
                            </center>";
                        break;
                    case "newUser":
                        verifyUrl = $"https://predictly.z13.web.core.windows.net/auth/new-user-setup?userid={user.Id}&token={token}";
                        subject = "Vaccination Management System - New User Invitation";
                        html =
                        $@" <center>
                                <img
                                style=""width: 40%""
                                src='https://docs.google.com/uc?id=1ornFZghAE9F3kNLxmMYNo5F9H0azVKU3'
                                alt=''
                                />
                            <h2>
                                New User Setup
                            </h2>
                            <br />
                                <a
                                style=""
                                    border-radius: 5px;
                                    color: white;
                                    background-color: rgb(179, 80, 204);
                                    padding: 15px;
                                    border: none;
                                    letter-spacing: 0.1rem;
                                    text-transform: uppercase;
                                    text-decoration: none;
                                ""
                                href=""{verifyUrl}""
                                >
                                Continue to Login
                                </a>
                            </center>";
                        break;
                    case "newUserSetup":
                        subject = "Password Setup Successfull";
                        html =
                        $@" <center>
                                <img
                                style=""width: 40%""
                                src='https://docs.google.com/uc?id=1GhZJQfcGeJhxZ0_kPh2MrfSMe9izMsi-'
                                alt=''
                                />
                            <h2>
                                Password of new user account <br />
                                setup successfully!
                            </h2>
                            <br />
                                <a
                                style=""
                                    border-radius: 5px;
                                    color: white;
                                    background-color: rgb(104, 107, 109);
                                    padding: 15px;
                                    border: none;
                                    letter-spacing: 0.1rem;
                                    text-transform: uppercase;
                                    text-decoration: none;
                                ""
                                href=""https://predictly.z13.web.core.windows.net/auth/login""
                                >
                                Continue to Login
                                </a>
                        </center>";
                        break;
                    case "passwordChanged":
                        subject = "Password Change Successfull";
                        html =
                        $@" <center><img
                                style=""width: 40%""
                                src='https://docs.google.com/uc?id=10uumtpFjMuE7CIXYiQjeKmNPMIhr1YkX'
                                alt=''
                                />
                            <h2>
                                Password change successfull!
                            </h2>
                            <br />
                                <a
                                style=""
                                    border-radius: 5px;
                                    color: white;
                                    background-color: rgba(245, 55, 91, 1);
                                    padding: 15px;
                                    border: none;
                                    letter-spacing: 0.1rem;
                                    text-transform: uppercase;
                                    text-decoration: none;
                                ""
                                href=""https://predictly.z13.web.core.windows.net/auth/login""
                                >
                                Continue to Login
                                </a><center>";
                        break;

                }

                _emailService.Send(
                    to: email,
                    subject: subject,
                    html: html
                );

            }
            catch (Exception ex)
            {
                _logger.LogError("Error occured in auth email sending", ex.Message);
                throw;
            }
        }

    }
}
