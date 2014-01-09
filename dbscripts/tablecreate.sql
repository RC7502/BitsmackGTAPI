
/****** Object:  Table [dbo].[Cardio]    Script Date: 1/9/2014 1:49:54 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Cardio](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[trandate] [smalldatetime] NOT NULL,
	[activity] [varchar](50) NOT NULL,
	[distance] [float] NOT NULL,
	[time] [float] NOT NULL,
 CONSTRAINT [PK_Cardio] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Pedometer]    Script Date: 1/9/2014 1:49:54 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Pedometer](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[steps] [int] NOT NULL,
	[sleep] [int] NOT NULL,
	[trandate] [smalldatetime] NOT NULL,
	[weight] [float] NOT NULL,
	[bodyfat] [float] NOT NULL,
 CONSTRAINT [PK_Pedometer] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
ALTER TABLE [dbo].[Pedometer] ADD  CONSTRAINT [DF_Pedometer_weight]  DEFAULT ((0)) FOR [weight]
GO
ALTER TABLE [dbo].[Pedometer] ADD  CONSTRAINT [DF_Pedometer_bodyfat]  DEFAULT ((0)) FOR [bodyfat]
GO
