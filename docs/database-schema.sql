-- Gym Membership Management System - Database Schema
-- Generated for SQL Server (LocalDB / Express / Full)
-- Run this script to create the database schema manually if needed

USE [GMMSDb];
GO

/****** Object:  Table [dbo].[Tbl_Member]    ******/
SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO
CREATE TABLE [dbo].[Tbl_Member](
	[MemberId] [int] IDENTITY(1,1) NOT NULL,
	[MemberCode] [nvarchar](50) NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
	[IsDeleted] [bit] NOT NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[UpdatedAt] [datetime2](7) NULL,
PRIMARY KEY CLUSTERED 
(
	[MemberId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
UNIQUE NONCLUSTERED 
(
	[MemberCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY];
GO

ALTER TABLE [dbo].[Tbl_Member] ADD  DEFAULT ((0)) FOR [IsDeleted];
GO
ALTER TABLE [dbo].[Tbl_Member] ADD  DEFAULT (getdate()) FOR [CreatedAt];
GO

/****** Object:  Table [dbo].[Tbl_Membership]    ******/
SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO
CREATE TABLE [dbo].[Tbl_Membership](
	[MembershipId] [int] IDENTITY(1,1) NOT NULL,
	[MemberId] [int] NOT NULL,
	[MembershipPlanId] [int] NOT NULL,
	[StartDate] [date] NOT NULL,
	[EndDate] [date] NOT NULL,
	[Status] [nvarchar](20) NOT NULL,
	[IsDeleted] [bit] NOT NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[UpdatedAt] [datetime2](7) NULL,
PRIMARY KEY CLUSTERED 
(
	[MembershipId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY];
GO

ALTER TABLE [dbo].[Tbl_Membership] ADD  DEFAULT ((0)) FOR [IsDeleted];
GO
ALTER TABLE [dbo].[Tbl_Membership] ADD  DEFAULT (getdate()) FOR [CreatedAt];
GO

ALTER TABLE [dbo].[Tbl_Membership]  WITH CHECK ADD  CONSTRAINT [FK_Membership_Member] FOREIGN KEY([MemberId])
REFERENCES [dbo].[Tbl_Member] ([MemberId]);
GO
ALTER TABLE [dbo].[Tbl_Membership] CHECK CONSTRAINT [FK_Membership_Member];
GO

ALTER TABLE [dbo].[Tbl_Membership]  WITH CHECK ADD  CONSTRAINT [FK_Membership_MembershipPlan] FOREIGN KEY([MembershipPlanId])
REFERENCES [dbo].[Tbl_MembershipPlan] ([MembershipPlanId]);
GO
ALTER TABLE [dbo].[Tbl_Membership] CHECK CONSTRAINT [FK_Membership_MembershipPlan];
GO

ALTER TABLE [dbo].[Tbl_Membership]  WITH CHECK ADD  CHECK  (([Status]='Cancelled' OR [Status]='Expired' OR [Status]='Active' OR [Status]='Pending'));
GO

/****** Object:  Table [dbo].[Tbl_MembershipPlan]    ******/
SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO
CREATE TABLE [dbo].[Tbl_MembershipPlan](
	[MembershipPlanId] [int] IDENTITY(1,1) NOT NULL,
	[PlanName] [nvarchar](100) NOT NULL,
	[Price] [decimal](18, 2) NOT NULL,
	[DurationDays] [int] NOT NULL,
	[Description] [nvarchar](500) NULL,
	[IsActive] [bit] NOT NULL,
	[IsDeleted] [bit] NOT NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[UpdatedAt] [datetime2](7) NULL,
	[PlanCode] [nvarchar](50) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[MembershipPlanId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY];
GO

ALTER TABLE [dbo].[Tbl_MembershipPlan] ADD  DEFAULT ((1)) FOR [IsActive];
GO
ALTER TABLE [dbo].[Tbl_MembershipPlan] ADD  DEFAULT ((0)) FOR [IsDeleted];
GO
ALTER TABLE [dbo].[Tbl_MembershipPlan] ADD  DEFAULT (getdate()) FOR [CreatedAt];
GO
ALTER TABLE [dbo].[Tbl_MembershipPlan] ADD  DEFAULT ('') FOR [PlanCode];
GO

/****** Object:  Table [dbo].[Tbl_Payment]    ******/
SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO
CREATE TABLE [dbo].[Tbl_Payment](
	[PaymentId] [int] IDENTITY(1,1) NOT NULL,
	[MembershipId] [int] NOT NULL,
	[PaymentMethodId] [int] NOT NULL,
	[Amount] [decimal](18, 2) NOT NULL,
	[SSPath] [nvarchar](500) NULL,
	[Status] [nvarchar](20) NOT NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[PaymentId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY];
GO

ALTER TABLE [dbo].[Tbl_Payment] ADD  DEFAULT (getdate()) FOR [CreatedAt];
GO

ALTER TABLE [dbo].[Tbl_Payment]  WITH CHECK ADD  CONSTRAINT [FK_Payment_Membership] FOREIGN KEY([MembershipId])
REFERENCES [dbo].[Tbl_Membership] ([MembershipId]);
GO
ALTER TABLE [dbo].[Tbl_Payment] CHECK CONSTRAINT [FK_Payment_Membership];
GO

ALTER TABLE [dbo].[Tbl_Payment]  WITH CHECK ADD  CONSTRAINT [FK_Payment_PaymentMethod] FOREIGN KEY([PaymentMethodId])
REFERENCES [dbo].[Tbl_PaymentMethod] ([PaymentMethodId]);
GO
ALTER TABLE [dbo].[Tbl_Payment] CHECK CONSTRAINT [FK_Payment_PaymentMethod];
GO

ALTER TABLE [dbo].[Tbl_Payment]  WITH CHECK ADD  CHECK  (([Status]='Failed' OR [Status]='Paid' OR [Status]='Pending'));
GO

/****** Object:  Table [dbo].[Tbl_PaymentMethod]    ******/
SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO
CREATE TABLE [dbo].[Tbl_PaymentMethod](
	[PaymentMethodId] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
	[IsActive] [bit] NOT NULL,
	[IsDeleted] [bit] NOT NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[UpdatedAt] [datetime2](7) NULL,
	[PaymentMethodCode] [nvarchar](50) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[PaymentMethodId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY];
GO

ALTER TABLE [dbo].[Tbl_PaymentMethod] ADD  DEFAULT ((1)) FOR [IsActive];
GO
ALTER TABLE [dbo].[Tbl_PaymentMethod] ADD  DEFAULT ((0)) FOR [IsDeleted];
GO
ALTER TABLE [dbo].[Tbl_PaymentMethod] ADD  DEFAULT (getdate()) FOR [CreatedAt];
GO
ALTER TABLE [dbo].[Tbl_PaymentMethod] ADD  DEFAULT ('') FOR [PaymentMethodCode];
GO