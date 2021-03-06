# Personal Stock Trader

Demo project for trading stocks on NYSE. For now it is implemented for trading only IBM stock, and provided data is 1 min. series.


[![Build status](https://dev.azure.com/amstanchev/PersonalStockTrader/_apis/build/status/PersonalStockTrader-Azure%20Web%20App%20for%20ASP.NET-CI)](https://dev.azure.com/amstanchev/PersonalStockTrader/_build/latest?definitionId=-1)

## Roles

* Visitor
* Administrator
* Account manager
* User with not confirmed account
* User with confirmed account

## Getting Started

Site guest (**visitor**) 
* can only visit Info pages (*Home*, *About Us*, *Privacy*, *Our Services*, *Contact Us*, *Register*, *Login*) 
		and can send e-mail from form to site owner.

Every user has to register, except regular data and starting balance for his account (more then 1000USD). After his account is confirmed by account manager, he can use the full functionality of the trade platform.
Every user pays trade fee for every trade and monthly commision for account maintenance.
Every registered user,administrator and account manager has own profile page.


**Administrator** - created from site owner
* Manage account managers.
* Create, update and delete account manager. 
* Manage sent to site e-mails from visitors. 
* Manage Hangfire dashboard.


**Account manager** - created from administrator
* Manage new accounts. 
* Confirm user accounts. After confirmation user receives automatically generated email.
* Manage confirmed clients
	- update their fees and balance
	- send them e-mails
	- activate, deactivate their accounts
	- can delete user.
* Sees clients statistics (graphics)
	- Paid total trade fees last 7 days 
	- Paid total monthly commissions last 6 months
	- New clients last 90 days
	- Money from commissions and fees last 90 days
	
		
**User with not confirmed account** - register
* Can only visit his profile page.


**User with confirmed account** - register
* Can see current IBM price (1 min series) -live updated data
* Can see live update 1 min chart for IBM stock
* Can buy or sell IBM stocks (depending of his account balance)
* Can see updated position profit/loss depending of market direction
* Can see history of his positions (can choose interval for them)
* Can generate report with closed positions
* Can see statistics of his paid trade and monthly fees (can choose interval for them) (graphics)
* Can see his profit/loss from closed positions (can choose interval for them) (graphics)

## Background processes

**Hangfire** has two registered jobs
* Every 1 min to collect data from external api - Alpha Vantage API and populate the database
* Every month to collect monthly commissions from every active account

**SignalR**
* Checks if there is new data collected from Hangfire in the Database and if so - updates stock price and time in user's trading platform

## Unit tests Code coverage

![Code coverage](https://github.com/astanchev/Personal-Stock-Trader/blob/master/Images/Code_Coverage.png)

## Template authors

- [Nikolay Kostov](https://github.com/NikolayIT)
- [Vladislav Karamfilov](https://github.com/vladislav-karamfilov)

## Template Layouts

* **Majestic Admin** - https://www.urbanui.com/
* **TemplateMo 545 FinanceBuisness** - https://templatemo.com/

## Data providers

* **Alpha Vantage API** - https://www.alphavantage.co
* **Time and date** - https://www.timeanddate.com/
* **Error page -** ***You are lost...*** by Brett Thurston - https://codepen.io/ohBretterson/pen/gjyrzB

## Used Frameworks

* ASP.Net Core 3.1
* SignalR
* Hangfire
* NUnit
* SendGrid
* Moment.js
* Chart.js
* Phantom.js
* Newtonsoft.Json
* Moq
* MockQueryable - https://github.com/romantitov/MockQueryable

## Used techniques
* MVC
* Repository pattern
* Services
* Web Api controllers + AJAX
* In-Memmory Cache
* Modul form /for sending e-mails from account manager to client/
* TempData message for sent email from site visitor

## Pictures

* Unregistered user

**Home**
![Unregistered user](https://github.com/astanchev/Personal-Stock-Trader/blob/master/Images/Home1.png)

**OurServices**
![Unregistered user](https://github.com/astanchev/Personal-Stock-Trader/blob/master/Images/OurServices.png) 

**Footer Contact Us**
![Unregistered user](https://github.com/astanchev/Personal-Stock-Trader/blob/master/Images/FooterContactUs.png)

* Registration form / login form

**Register**
![Registration form](https://github.com/astanchev/Personal-Stock-Trader/blob/master/Images/Register.png) 

**Login**
![login form](https://github.com/astanchev/Personal-Stock-Trader/blob/master/Images/Login.png)

* Administrator

**Administrator Home**
![Administrator](https://github.com/astanchev/Personal-Stock-Trader/blob/master/Images/AdministratorHome.png) 

**Manage Emails**
![Administrator](https://github.com/astanchev/Personal-Stock-Trader/blob/master/Images/ManageEmails.png)

* Customer manager

**Manager Clients**
![Customer manager](https://github.com/astanchev/Personal-Stock-Trader/blob/master/Images/ManagerClients.png) 

**Manage Client**
![Customer manager](https://github.com/astanchev/Personal-Stock-Trader/blob/master/Images/ManageClient.png)

* Registered user with unapproved account

**New Account**
![Registered user with unapproved account](https://github.com/astanchev/Personal-Stock-Trader/blob/master/Images/newAccount.png)

* Registered user with approved account

**UserHome**
![User Home](https://github.com/astanchev/Personal-Stock-Trader/blob/master/Images/UserHome.png) 

**User Statistic**
![User Statistic](https://github.com/astanchev/Personal-Stock-Trader/blob/master/Images/UserStatistic.png) 

**Trade History**
![Trade History](https://github.com/astanchev/Personal-Stock-Trader/blob/master/Images/TradeHistory.png)

## License

This project is licensed under the MIT License - see the [LICENSE.md](LICENSE.md) file for details

## Author

- [Atanas Stanchev](https://github.com/astanchev)
