IF (SELECT OBJECT_ID('ext.ITSRETAILPARAMETERS')) IS NULL
BEGIN
CREATE TABLE [ext].[ITSRETAILPARAMETERS](
	[DATAAREAID] [nvarchar](4) NOT NULL DEFAULT('dat'),
	[ITSSUPPORTUSERID] [nvarchar](100) NOT NULL DEFAULT(''),
	[ITSB2CEXTENSIONAPPCLIENTID] [nvarchar](100) NOT NULL DEFAULT(''),
	[ITSCLIENTSECRET] [nvarchar](100) NOT NULL DEFAULT(''),
	[ITSAPPLICATIONCLIENTID] [nvarchar](100) NOT NULL DEFAULT(''),
	[ITSB2CTENANTID] [nvarchar](100) NOT NULL DEFAULT(''),
 CONSTRAINT [I_ITSRETAILPARAMETERS] PRIMARY KEY CLUSTERED 
(
	[DATAAREAID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
END
GO
GRANT SELECT,INSERT,UPDATE,DELETE ON [ext].[ITSRETAILPARAMETERS] To [UsersRole]
GO
GRANT SELECT,INSERT,UPDATE,DELETE ON [ext].[ITSRETAILPARAMETERS] To [DeployExtensibilityRole]
GO
GRANT SELECT,INSERT,UPDATE,DELETE ON [ext].[ITSRETAILPARAMETERS] To [DataSyncUsersRole]
GO


