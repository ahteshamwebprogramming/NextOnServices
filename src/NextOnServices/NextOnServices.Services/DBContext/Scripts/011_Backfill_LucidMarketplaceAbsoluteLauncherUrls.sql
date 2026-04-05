-- Set @PublicWebBaseUrl to the canonical public Lucid Marketplace web base URL, for example: https://nexton.us
DECLARE @PublicWebBaseUrl NVARCHAR(500) = LTRIM(RTRIM(N''));
SET @PublicWebBaseUrl = REPLACE(@PublicWebBaseUrl, NCHAR(9), N'');
SET @PublicWebBaseUrl = RTRIM(@PublicWebBaseUrl);

IF NULLIF(@PublicWebBaseUrl, N'') IS NULL
BEGIN
    THROW 50000, 'Set @PublicWebBaseUrl to the canonical Lucid Marketplace web base URL before running this script.', 1;
END

IF RIGHT(@PublicWebBaseUrl, 1) = N'/'
BEGIN
    SET @PublicWebBaseUrl = LEFT(@PublicWebBaseUrl, LEN(@PublicWebBaseUrl) - 1);
END

UPDATE pu
SET pu.Url = @PublicWebBaseUrl + pu.Url
FROM [dbo].[ProjectsUrl] pu
INNER JOIN [dbo].[Projects] p
    ON p.ProjectId = pu.PID
WHERE p.ProjectFrom = N'LucidMarketplace'
  AND pu.Url LIKE N'/VT/LucidMarketplace/LaunchRespondent%'
  AND pu.Url NOT LIKE N'http://%'
  AND pu.Url NOT LIKE N'https://%';

UPDATE pm
SET pm.OLink = @PublicWebBaseUrl + pm.OLink
FROM [dbo].[ProjectMapping] pm
INNER JOIN [dbo].[Projects] p
    ON p.ProjectId = pm.ProjectID
WHERE p.ProjectFrom = N'LucidMarketplace'
  AND pm.OLink LIKE N'/VT/LucidMarketplace/LaunchRespondent%'
  AND pm.OLink NOT LIKE N'http://%'
  AND pm.OLink NOT LIKE N'https://%';
