# 🍕 Little Caesars Waste & Loss Tracker

A full-stack web application built for kitchen staff and managers at **Little Caesars Saddle Creek** to log, track, and analyze food waste in real time.

> Built with C# .NET 9, AWS RDS MySQL, AWS S3, and deployed on AWS Elastic Beanstalk.

**Live Demo:** http://littlecaesarswastetracker-env.eba-3hixjq7q.us-east-1.elasticbeanstalk.com/login.html

---

## 📋 Features

- **PIN Authentication** — Staff select their name and enter a 4-digit PIN (mirrors real restaurant POS systems)
- **Quick Waste Logging** — Tap any menu item (HNR, Pizzas, Sides, Wings), set quantity, select reason
- **Real-Time Summary** — Total waste value, breakdowns by item and reason
- **30-Day History** — Daily waste totals for the last 30 days
- **End-of-Night Photo** — Upload shift photo to AWS S3 for accountability
- **Export Report** — Download a plain-text waste report for any day
- **Role-Based Access** — Manager, Assistant Manager, Shift Lead

---

## 🛠 Tech Stack

| Layer | Technology |
|-------|------------|
| Backend | C# .NET 9, ASP.NET Core Web API |
| Architecture | Clean Architecture (Core / Infrastructure / API) |
| Database | AWS RDS MySQL 8.4 |
| File Storage | AWS S3 |
| Deployment | AWS Elastic Beanstalk (.NET 9 on Linux) |
| Frontend | HTML, CSS, JavaScript (no framework) |
| ORM | Entity Framework Core 9 + Pomelo MySQL |

---

## 🚀 Deployment Instructions

Follow these steps exactly to deploy this application from scratch.

### Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9)
- AWS Account with access to RDS, S3, IAM, and Elastic Beanstalk
- Git

---

### Step 1 — Clone the Repository

```bash
git clone https://github.com/YOUR_USERNAME/LittleCaesars-Waste-Tracker.git
cd LittleCaesars-Waste-Tracker
```

---

### Step 2 — Set Up AWS RDS MySQL

1. Go to **AWS Console → RDS → Create database**
2. Engine: **MySQL 8.x**, Template: **Free tier**
3. DB identifier: `littlecaesars-db`
4. Master username: `admin`, set a password
5. Under **Connectivity** → Public access: **Yes**
6. Create a new security group or add an inbound rule for **port 3306**
7. Additional config → Initial database name: `littlecaesarsdb`
8. Click **Create database** and wait for status: **Available**

**Seed the database via AWS CloudShell:**

```bash
mysql -h YOUR_RDS_ENDPOINT -P 3306 --ssl-ca /certs/global-bundle.pem -u admin -p
```

Then run the seed SQL:

```sql
USE littlecaesarsdb;

CREATE TABLE Users (Id INT AUTO_INCREMENT PRIMARY KEY, Name VARCHAR(100) NOT NULL, PIN VARCHAR(4) NOT NULL, Role VARCHAR(50) NOT NULL, IsActive TINYINT(1) NOT NULL DEFAULT 1);
CREATE TABLE MenuItems (Id INT AUTO_INCREMENT PRIMARY KEY, Name VARCHAR(100) NOT NULL, CustomerPrice DECIMAL(10,2) NOT NULL, IsActive TINYINT(1) NOT NULL DEFAULT 1, IsCustom TINYINT(1) NOT NULL DEFAULT 0);
CREATE TABLE WasteReasons (Id INT AUTO_INCREMENT PRIMARY KEY, Reason VARCHAR(100) NOT NULL);
CREATE TABLE WasteLogs (Id INT AUTO_INCREMENT PRIMARY KEY, UserId INT NOT NULL, MenuItemId INT NOT NULL, WasteReasonId INT NOT NULL, Quantity INT NOT NULL, UnitPrice DECIMAL(10,2) NOT NULL, TotalCost DECIMAL(10,2) NOT NULL, Shift VARCHAR(20) NOT NULL, Notes TEXT NULL, LoggedAt DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP, FOREIGN KEY (UserId) REFERENCES Users(Id), FOREIGN KEY (MenuItemId) REFERENCES MenuItems(Id), FOREIGN KEY (WasteReasonId) REFERENCES WasteReasons(Id));
CREATE TABLE DailySummaries (Id INT AUTO_INCREMENT PRIMARY KEY, Date DATETIME NOT NULL, TotalWasteValue DECIMAL(10,2) NOT NULL, PhotoUrl VARCHAR(500) NULL, SubmittedByUserId INT NOT NULL, SubmittedAt DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP, FOREIGN KEY (SubmittedByUserId) REFERENCES Users(Id));
CREATE TABLE __EFMigrationsHistory (MigrationId VARCHAR(150) NOT NULL PRIMARY KEY, ProductVersion VARCHAR(32) NOT NULL);

INSERT INTO WasteReasons (Reason) VALUES ('Burnt'),('Dropped'),('Expired'),('Wrong Order'),('Overproduced'),('Return'),('Quality Issue');
INSERT INTO Users (Name, PIN, Role, IsActive) VALUES ('Khan N.','1234','Manager',1),('Angel D.','2345','AssistantManager',1),('Wal H.','3456','ShiftLead',1),('Ana O.','4567','ShiftLead',1);
INSERT INTO MenuItems (Name, CustomerPrice, IsActive, IsCustom) VALUES ('Classic Pepperoni',6.99,1,0),('Classic Cheese',6.99,1,0),('3 Meat Treat',12.49,1,0),('5 Meat Feast',13.99,1,0),('Ultimate Supreme',13.99,1,0),('Crazy Bread',4.49,1,0),('Caesar Wings',9.99,1,0),('Other (Custom)',0.00,1,1);
```

---

### Step 3 — Set Up AWS S3

1. Go to **AWS Console → S3 → Create bucket**
2. Bucket name: `littlecaesars-waste-photos`
3. Region: `us-east-1`
4. Uncheck **Block all public access**
5. Add this bucket policy under **Permissions → Bucket policy**:

```json
{
  "Version": "2012-10-17",
  "Statement": [{
    "Sid": "PublicReadGetObject",
    "Effect": "Allow",
    "Principal": "*",
    "Action": "s3:GetObject",
    "Resource": "arn:aws:s3:::littlecaesars-waste-photos/*"
  }]
}
```

6. Go to **IAM → Users → Create user** named `littlecaesars-app`
7. Attach policy: `AmazonS3FullAccess`
8. Create **Access Key** → save the Access Key ID and Secret Access Key

---

### Step 4 — Configure appsettings.json

Open `LittleC.API/appsettings.json` and fill in your values:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_RDS_ENDPOINT;Port=3306;Database=littlecaesarsdb;User=admin;Password=YOUR_PASSWORD;SslMode=Required;"
  },
  "AWS": {
    "AccessKey": "YOUR_ACCESS_KEY",
    "SecretKey": "YOUR_SECRET_KEY",
    "Region": "us-east-1",
    "BucketName": "littlecaesars-waste-photos"
  },
  "Logging": {
    "LogLevel": { "Default": "Information", "Microsoft.AspNetCore": "Warning" }
  },
  "AllowedHosts": "*"
}
```

---

### Step 5 — Update Frontend API URL

In all 4 HTML files inside `LittleC.API/wwwroot/`, update the API constant to your Elastic Beanstalk URL:

```bash
cd LittleC.API/wwwroot
sed -i '' 's|http://localhost:5013|YOUR_ELASTIC_BEANSTALK_URL|g' login.html home.html wastelog.html summary.html
```

---

### Step 6 — Publish the Application

```bash
cd LittleC.API
dotnet publish -c Release -o ./publish
cp -r .ebextensions ./publish/
cd publish
zip -r ../LittleCaesars-deploy.zip .
```

---

### Step 7 — Deploy to Elastic Beanstalk

1. Go to **AWS Console → Elastic Beanstalk → Create application**
2. Application name: `LittleCaesarsWasteTracker`
3. Create new environment → **Web server environment**
4. Platform: **.NET 9 on Linux** (64-bit Amazon Linux 2023)
5. Application code: **Upload your code** → upload `LittleCaesars-deploy.zip`
6. Configure service access:
   - Service role: `aws-elasticbeanstalk-service-role`
   - EC2 instance profile: `aws-elasticbeanstalk-ec2-role`
7. Instance type: **t3.micro** (free tier)
8. Click **Submit** and wait ~10 minutes for **Health: Ok**

**After deployment**, add the Elastic Beanstalk security group to your RDS inbound rules:
- Go to **EC2 → Security Groups** → find `awseb-e-xxxxx-stack-AWSEBSecurityGroup`
- Add it to your RDS instance security groups via **RDS → Modify**

---

### Step 8 — Access the App

```
http://YOUR_EB_URL/login.html
```

Default login credentials:
| Name | PIN | Role |
|------|-----|------|
| Khan N. | 1234 | Manager |
| Angel D. | 2345 | Assistant Manager |
| Wal H. | 3456 | Shift Lead |
| Ana O. | 4567 | Shift Lead |

---

## 📁 Project Structure

```
LittleC/
├── LittleC.Core/              ← Models (User, MenuItem, WasteLog, etc.)
├── LittleC.Infrastructure/    ← Database (EF Core, S3 Service, Seeder)
│   ├── Data/
│   │   ├── AppDbContext.cs
│   │   ├── AppDbContextFactory.cs
│   │   └── S3Service.cs
│   └── Seed/
│       └── DataSeeder.cs
└── LittleC.API/               ← REST API + Frontend
    ├── Controllers/
    │   ├── AuthController.cs
    │   ├── MenuItemsController.cs
    │   ├── WasteLogController.cs
    │   └── SummaryController.cs
    ├── wwwroot/               ← Frontend HTML files
    │   ├── login.html
    │   ├── home.html
    │   ├── wastelog.html
    │   └── summary.html
    └── Program.cs
```

---

## 🌐 API Endpoints

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/auth/users` | Get all active staff |
| POST | `/api/auth/login` | Authenticate with PIN |
| GET | `/api/menuitems` | Get all menu items |
| GET | `/api/wastelog/reasons` | Get waste reasons |
| GET | `/api/wastelog/today` | Get today's logs |
| POST | `/api/wastelog` | Log a waste entry |
| DELETE | `/api/wastelog/{id}` | Delete a log entry |
| GET | `/api/summary/today` | Get today's summary |
| GET | `/api/summary/history` | Get 30-day history |
| POST | `/api/summary/upload-photo` | Upload shift photo to S3 |

---

## 👤 Author

**Angel Chairez** — Solo Developer  
5-Year Manager, Little Caesars Saddle Creek  
CSCI 4650 — Spring 2026
