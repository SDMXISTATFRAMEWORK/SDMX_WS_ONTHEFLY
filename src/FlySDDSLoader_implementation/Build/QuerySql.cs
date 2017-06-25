using FlyMapping.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FlySDDSLoader_implementation.Build
{
    /// <summary>
    /// All Query Sql for retreive data in OTF v2.0
    /// </summary>
    public class QuerySql
    {

        internal static string MSGetCodelist(ref List<IParameterValue> parameter)
        {
            string dec1 = string.Format(@"
            declare @CodelistCode varchar(max)= '{0}'
	        declare @CodelistAgencyId varchar(max)= '{1}' 
	        declare @CodelistVersion varchar(max)= '{2}' 
 
	        declare @DSDCode varchar(max)= '{3}' 
	        declare @DSDAgencyId varchar(max)= '{4}' 
	        declare @DSDVersion varchar(max)= '{5}' 
    
	        declare @ConceptSchemeCode varchar(max)= '{6}' 
	        declare @ConceptSchemeAgencyId varchar(max)= '{7}'
	        declare @ConceptSchemeVersion varchar(max)= '{8}' 

	        declare @IsStub bit={9}",
            (parameter.Exists(p=>p.Item=="CodelistCode")?parameter.Find(p=>p.Item=="CodelistCode").Value.ToString():""),
            (parameter.Exists(p=>p.Item=="CodelistAgencyId")?parameter.Find(p=>p.Item=="CodelistAgencyId").Value.ToString():""),
            (parameter.Exists(p=>p.Item=="CodelistVersion")?parameter.Find(p=>p.Item=="CodelistVersion").Value.ToString():""),
            (parameter.Exists(p=>p.Item=="DSDCode")?parameter.Find(p=>p.Item=="DSDCode").Value.ToString():""),
            (parameter.Exists(p=>p.Item=="DSDAgencyId")?parameter.Find(p=>p.Item=="DSDAgencyId").Value.ToString():""),
            (parameter.Exists(p=>p.Item=="DSDVersion")?parameter.Find(p=>p.Item=="DSDVersion").Value.ToString():""),
            (parameter.Exists(p=>p.Item=="ConceptSchemeCode")?parameter.Find(p=>p.Item=="ConceptSchemeCode").Value.ToString():""),
            (parameter.Exists(p=>p.Item=="ConceptSchemeAgencyId")?parameter.Find(p=>p.Item=="ConceptSchemeAgencyId").Value.ToString():""),
            (parameter.Exists(p=>p.Item=="ConceptSchemeVersion")?parameter.Find(p=>p.Item=="ConceptSchemeVersion").Value.ToString():""),
            (parameter.Exists(p => p.Item == "IsStub") ? parameter.Find(p => p.Item == "IsStub").Value.ToString() : "0"));
            return dec1 + @"
declare @CLList table
(
	CodelistID bigint,
	CodelistCode varchar(max),
	AGENCY varchar(max), 
	CodelistVersion varchar(max),
	lang varchar(max),
	descr varchar(max)
)

create table #CLITEMList 
(
	CodelistID bigint,
	ItemId bigint,
	ItemCode varchar(max),
	lang varchar(max),
	descr varchar(max),
	ParentCodeID bigint,
	ParentCode varchar(max),
	Livel int,
	Ordinamento varchar(max)
	
)

INSERT INTO @CLList 	
select DISTINCT CODELIST.CL_ID as CodelistId, ARTEFACT.ID as CodelistCode, ARTEFACT.AGENCY, LTRIM(STR(ARTEFACT.VERSION1)) + '.' + LTRIM(STR(ARTEFACT.VERSION2)) as SdmxVersion,
LANGUAGE as lang, TEXT as descr 


from CODELIST
inner join ARTEFACT on ARTEFACT.ART_ID= CODELIST.CL_ID
inner join LOCALISED_STRING on ARTEFACT.ART_ID= LOCALISED_STRING.ART_ID
LEFT join COMPONENT on CODELIST.CL_ID = COMPONENT.CL_ID
LEFT join ARTEFACT as ARTDSD on ARTDSD.ART_ID= COMPONENT.DSD_ID

LEFT JOIN CONCEPT on CONCEPT.CON_ID= COMPONENT.CON_ID
LEFT JOIN ARTEFACT as ARTConceptScheme on ARTConceptScheme.ART_ID = CONCEPT.CON_SCH_ID
--Where Codelist Code
where LOCALISED_STRING.TYPE='Name' AND
ARTEFACT.ID = case when LEN(@CodelistCode)>0 then @CodelistCode else ARTEFACT.ID end
--Codelist Agency e Version
AND ARTEFACT.AGENCY = case when LEN(@CodelistAgencyId)>0 then @CodelistAgencyId else ARTEFACT.AGENCY end
AND  LTRIM(STR(ARTEFACT.VERSION1)) + '.' + LTRIM(STR(ARTEFACT.VERSION2))= case when LEN(@CodelistVersion)>0 then @CodelistVersion else LTRIM(STR(ARTEFACT.VERSION1)) + '.' + LTRIM(STR(ARTEFACT.VERSION2)) end
-- ConceptScheme
AND ARTConceptScheme.ID =
case when LEN(@ConceptSchemeCode)>0 then @ConceptSchemeCode else ARTConceptScheme.ID end
--ConceptScheme Agency e Version
AND ARTConceptScheme.AGENCY = case when LEN(@ConceptSchemeAgencyId)>0 then @ConceptSchemeAgencyId else ARTConceptScheme.AGENCY end
AND  LTRIM(STR(ARTConceptScheme.VERSION1)) + '.' + LTRIM(STR(ARTConceptScheme.VERSION2))= case when LEN(@ConceptSchemeVersion)>0 then @ConceptSchemeVersion else LTRIM(STR(ARTConceptScheme.VERSION1)) + '.' + LTRIM(STR(ARTConceptScheme.VERSION2)) end

-- DSDCode
AND ARTDSD.ID = 
case when LEN(@DSDCode)>0 then @DSDCode else ARTDSD.ID end
--DSD Agency e Version
AND ARTDSD.AGENCY = case when LEN(@DSDAgencyId)>0 then @DSDAgencyId else ARTDSD.AGENCY end
AND  LTRIM(STR(ARTDSD.VERSION1)) + '.' + LTRIM(STR(ARTDSD.VERSION2))= case when LEN(@DSDVersion)>0 then @DSDVersion else LTRIM(STR(ARTDSD.VERSION1)) + '.' + LTRIM(STR(ARTDSD.VERSION2)) end


IF @IsStub=0
BEGIN
INSERT INTO #CLITEMList 
select DISTINCT CODELIST.CL_ID, ITEM.ITEM_ID,  ITEM.ID, LANGUAGE as lang, TEXT as txt, PARENT_CODE_ID,
(select ITEM.ID from ITEM where ITEM.ITEM_ID=PARENT_CODE_ID) as ParentCode,
Case when PARENT_CODE_ID is null then 0 else 1 end as livel,
Case when PARENT_CODE_ID is null then LTRIM(STR(ITEM.ITEM_ID)) else (select LTRIM(STR(ITEM.ITEM_ID)) from ITEM where ITEM.ITEM_ID=PARENT_CODE_ID) + '-' + LTRIM(STR(ITEM.ITEM_ID)) end as ordinamento
from CODELIST
inner join DSD_CODE on DSD_CODE.CL_ID=  CODELIST.CL_ID
inner join ITEM on DSD_CODE.LCD_ID= ITEM.ITEM_ID
inner join LOCALISED_STRING on ITEM.ITEM_ID= LOCALISED_STRING.ITEM_ID
where  CODELIST.CL_ID in (select DISTINCT CodelistID from  @CLList)
order by CODELIST.CL_ID
END



declare @RicQuanti int=1
SET NOCOUNT ON;

WHILE(@RicQuanti>0)
BEGIN
	
	UPDATE #CLITEMList 
	set Livel=Livel+1,
	Ordinamento = (select  DISTINCT pa.Ordinamento from #CLITEMList as pa where pa.ItemId = #CLITEMList.ParentCodeID) +'-' + LTRIM(STR(ItemId))
	where ParentCode is not null
	AND (select  DISTINCT pa.Ordinamento from #CLITEMList as pa where pa.ItemId = #CLITEMList.ParentCodeID)
	<> REPLACE(Ordinamento,'-' + LTRIM(STR(ItemId)),'')
	set @RicQuanti=@@ROWCOUNT
	
END
SET NOCOUNT OFF;

create table #T
(
	Tag int,
	Parent int null,
	[CodeLists!1!xmlns] varchar(max) null,
	[CodeList!2!Order!hide] varchar(max) null,
	[CodeList!2!Order2!hide] varchar(max) null,
	[CodeList!2!Code] varchar(max) null,
	[CodeList!2!AgencyID] varchar(max) null,
	[CodeList!2!Version] varchar(max) null,
	
	[Name!3!LocaleIsoCode] char(2) null,
	[Name!3!!cdata] varchar(max) null,

)
declare @i int
declare @si0 varchar(max)
declare @si1 varchar(max)
declare @si2 varchar(max)
declare @siParent varchar(max)
declare @execstr varchar(max)
declare @maxdepth int;
set @maxdepth =(select MAX(Livel) from #CLITEMList) +1

set @execstr = ''
set @i = 1
while (@i <= @maxdepth)
begin
	set @si1 = cast(2*@i+2 as varchar(max))
	set @si2 = cast(2*@i+3 as varchar(max))
	if (len(@execstr) > 0)
		set @execstr = @execstr + ','
	set @execstr = @execstr + '
[Code!'+@si1+'!value] varchar(max) null,
[Code!'+@si1+'!ID!hide] int null,
[Name!'+@si2+'!LocaleIsoCode] char(2) null,
[Name!'+@si2+'!!cdata] varchar(max) null'
	set @i = @i+1
end

if (len(@execstr) > 0)
--	print('alter table #T add '+@execstr)
	exec('alter table #T add '+@execstr)

INSERT INTO #T (Tag,[CodeLists!1!xmlns]) Values (1,'http://istat.it/OnTheFly')

INSERT INTO #T (Tag,Parent,[CodeList!2!Order!hide], [CodeList!2!Code], [CodeList!2!AgencyID], [CodeList!2!Version]) 
select distinct 2,1,CodelistID, CodelistCode
, AGENCY, CodelistVersion 
from @CLList

INSERT INTO #T (Tag,Parent,[CodeList!2!Order!hide], [CodeList!2!Code],[Name!3!LocaleIsoCode],[Name!3!!cdata]) 
select distinct 3,2,CodelistID, CodelistCode, lang,descr from @CLList




set @i = 1
while (@i <= @maxdepth)
begin
	set @si1 = cast(2*@i+2 as varchar(max))
	set @si2 = cast(2*@i+3 as varchar(max))
	set @siParent= cast(2*@i as varchar(max))
	
	EXEC('INSERT INTO #T (Tag,Parent,[CodeList!2!Order!hide],[CodeList!2!Order2!hide],
	[Code!'+@si1+'!value],[Code!'+@si1+'!ID!hide]) 
	select DISTINCT '+@si1+' as Tag, '+@siParent+' as Parent, CodelistID, Ordinamento, 
	ItemCode, ItemId
	from #CLITEMList where Livel='+@i + '-1')

	EXEC('INSERT INTO #T (Tag,Parent,[CodeList!2!Order!hide],[CodeList!2!Order2!hide],
	[Code!'+@si1+'!value],[Code!'+@si1+'!ID!hide],
	[Name!'+@si2+'!LocaleIsoCode], [Name!'+@si2+'!!cdata]) 
	select distinct '+@si2+' as Tag, '+@si1+' as Parent,   CodelistID, Ordinamento + '' '' +lang,
	ItemCode, ItemId,
	lang, descr 
	from #CLITEMList where Livel='+@i + '-1')
	set @i = @i+1
end

select * from #T order by [CodeList!2!Order!hide], [CodeList!2!Order2!hide],[Name!3!LocaleIsoCode]
FOR XML EXPLICIT
            ";
        }

        internal static string MSGetConceptScheme(ref List<IParameterValue> parameter)
        {
            string dec1 = string.Format(@"
            declare @CodelistCode varchar(max)= '{0}'
	        declare @CodelistAgencyId varchar(max)= '{1}' 
	        declare @CodelistVersion varchar(max)= '{2}' 
 
	        declare @DSDCode varchar(max)= '{3}' 
	        declare @DSDAgencyId varchar(max)= '{4}' 
	        declare @DSDVersion varchar(max)= '{5}' 
    
	        declare @ConceptSchemeCode varchar(max)= '{6}' 
	        declare @ConceptSchemeAgencyId varchar(max)= '{7}'
	        declare @ConceptSchemeVersion varchar(max)= '{8}' 

	        declare @IsStub bit={9}",
            (parameter.Exists(p => p.Item == "CodelistCode") ? parameter.Find(p => p.Item == "CodelistCode").Value.ToString() : ""),
            (parameter.Exists(p => p.Item == "CodelistAgencyId") ? parameter.Find(p => p.Item == "CodelistAgencyId").Value.ToString() : ""),
            (parameter.Exists(p => p.Item == "CodelistVersion") ? parameter.Find(p => p.Item == "CodelistVersion").Value.ToString() : ""),
            (parameter.Exists(p => p.Item == "DSDCode") ? parameter.Find(p => p.Item == "DSDCode").Value.ToString() : ""),
            (parameter.Exists(p => p.Item == "DSDAgencyId") ? parameter.Find(p => p.Item == "DSDAgencyId").Value.ToString() : ""),
            (parameter.Exists(p => p.Item == "DSDVersion") ? parameter.Find(p => p.Item == "DSDVersion").Value.ToString() : ""),
            (parameter.Exists(p => p.Item == "ConceptSchemeCode") ? parameter.Find(p => p.Item == "ConceptSchemeCode").Value.ToString() : ""),
            (parameter.Exists(p => p.Item == "ConceptSchemeAgencyId") ? parameter.Find(p => p.Item == "ConceptSchemeAgencyId").Value.ToString() : ""),
            (parameter.Exists(p => p.Item == "ConceptSchemeVersion") ? parameter.Find(p => p.Item == "ConceptSchemeVersion").Value.ToString() : ""),
            (parameter.Exists(p => p.Item == "IsStub") ? parameter.Find(p => p.Item == "IsStub").Value.ToString() : "0"));
            return dec1 + @"
declare @CSList table
(
	ConceptSchemeID varchar(max),
	AGENCY varchar(max), 
	ConceptSchemeVersion varchar(max),
	lang varchar(max),
	descr varchar(max),
	ConceptCode varchar(max),
	ConceptID varchar(max)
	
)

INSERT INTO @CSList 	
select distinct ARTEFACT.ID as ConceptSchemeID, ARTEFACT.AGENCY, LTRIM(STR(ARTEFACT.VERSION1)) + '.' + LTRIM(STR(ARTEFACT.VERSION2)) as SdmxVersion, LANGUAGE as lang, TEXT as descr,

COMPONENT.ID as ConceptCode,COMPONENT.CON_ID as ConceptID

from CONCEPT_SCHEME
inner join ARTEFACT on ARTEFACT.ART_ID= CONCEPT_SCHEME.CON_SCH_ID
inner join LOCALISED_STRING on ARTEFACT.ART_ID= LOCALISED_STRING.ART_ID
inner join CONCEPT on CONCEPT_SCHEME.CON_SCH_ID= CONCEPT.CON_SCH_ID
inner join COMPONENT on COMPONENT.CON_ID= CONCEPT.CON_ID
LEFT join ARTEFACT as ARTDSD on ARTDSD.ART_ID= COMPONENT.DSD_ID
LEFT join ARTEFACT as ARTCodelist on ARTCodelist.ART_ID= COMPONENT.CL_ID
--Where ConceptScheme Code
where LOCALISED_STRING.TYPE='Name' And
ARTEFACT.ID = case when LEN(@ConceptSchemeCode)>0 then @ConceptSchemeCode else ARTEFACT.ID end
--ConceptScheme Agency e Version
AND ARTEFACT.AGENCY = case when LEN(@ConceptSchemeAgencyId)>0 then @ConceptSchemeAgencyId else ARTEFACT.AGENCY end
AND  LTRIM(STR(ARTEFACT.VERSION1)) + '.' + LTRIM(STR(ARTEFACT.VERSION2))= case when LEN(@ConceptSchemeVersion)>0 then @ConceptSchemeVersion else LTRIM(STR(ARTEFACT.VERSION1)) + '.' + LTRIM(STR(ARTEFACT.VERSION2)) end

-- DSDCode
AND ARTDSD.ID = 
case when LEN(@DSDCode)>0 then @DSDCode else ARTDSD.ID end
--DSD Agency e Version
AND ARTDSD.AGENCY = case when LEN(@DSDAgencyId)>0 then @DSDAgencyId else ARTDSD.AGENCY end
AND  LTRIM(STR(ARTDSD.VERSION1)) + '.' + LTRIM(STR(ARTDSD.VERSION2))= case when LEN(@DSDVersion)>0 then @DSDVersion else LTRIM(STR(ARTDSD.VERSION1)) + '.' + LTRIM(STR(ARTDSD.VERSION2)) end

-- Codelist Code
AND (ARTCodelist.ID =
case when LEN(@CodelistCode)>0 then @CodelistCode else ARTCodelist.ID end
OR ARTCodelist.ID is NULL) 
--Codelist Agency e Version
AND (ARTCodelist.AGENCY = case when LEN(@CodelistAgencyId)>0 then @CodelistAgencyId else ARTCodelist.AGENCY end
	OR ARTCodelist.AGENCY is NULL) 
AND  (LTRIM(STR(ARTCodelist.VERSION1)) + '.' + LTRIM(STR(ARTCodelist.VERSION2))= case when LEN(@CodelistVersion)>0 then @CodelistVersion else LTRIM(STR(ARTCodelist.VERSION1)) + '.' + LTRIM(STR(ARTCodelist.VERSION2)) end
	OR LTRIM(STR(ARTCodelist.VERSION1)) + '.' + LTRIM(STR(ARTCodelist.VERSION2)) is NULL)


declare @ConceptNames table
(
	ConceptID varchar(max),
	lang varchar(max),
	descr varchar(max)
)

insert into @ConceptNames
SELECT DISTINCT CSList.ConceptID,
	LOCALISED_STRING.LANGUAGE, LOCALISED_STRING.TEXT
	from @CSList as CSList
	inner join ITEM on CSList.ConceptID= ITEM.ITEM_ID
	inner join LOCALISED_STRING on ITEM.ITEM_ID= LOCALISED_STRING.ITEM_ID
	where LOCALISED_STRING.TYPE='Name'


declare @CSXML table
(
	Tag int,
	Parent int null,
	[Concepts!1!xmlns] varchar(max) null,
	[ConceptScheme!2!Order!hide] varchar(max) null,
	[ConceptScheme!2!Order2!hide] varchar(max) null,
	[ConceptScheme!2!Code] varchar(max) null,
	[ConceptScheme!2!Agency] varchar(max) null,
	[ConceptScheme!2!Version] varchar(max) null,
	[Name!3!LocaleIsoCode] char(2) null,
	[Name!3!!cdata] varchar(max) null,
	[Concept!4!Code] varchar(max) null,
	--[Concept!4!Type] varchar(max) null,
	--[Concept!4!assignmentStatus] varchar(max) null,
	--[Concept!4!attachmentLevel] varchar(max) null,
	[Name!5!LocaleIsoCode] char(2) null,
	[Name!5!!cdata] varchar(max) null
)

INSERT INTO @CSXML (Tag,[Concepts!1!xmlns],[ConceptScheme!2!Order!hide],[ConceptScheme!2!Order2!hide]) Values (1,'http://istat.it/OnTheFly',0,0)

INSERT INTO @CSXML (Tag,Parent, [ConceptScheme!2!Order!hide],[ConceptScheme!2!Order2!hide] ,
	[ConceptScheme!2!Code] ,	[ConceptScheme!2!Agency] ,	[ConceptScheme!2!Version] 
	)
	SELECT DISTINCT 2,1,ConceptSchemeID+AGENCY+ConceptSchemeVersion, 0,
	ConceptSchemeID, AGENCY, ConceptSchemeVersion
	from @CSList

INSERT INTO @CSXML (Tag,Parent, [ConceptScheme!2!Order!hide],[ConceptScheme!2!Order2!hide] ,
	[ConceptScheme!2!Code] ,	[ConceptScheme!2!Agency] ,	[ConceptScheme!2!Version] ,
	[Name!3!LocaleIsoCode], [Name!3!!cdata] )  
	SELECT DISTINCT 3,2,ConceptSchemeID+AGENCY+ConceptSchemeVersion, 0,
	ConceptSchemeID, AGENCY, ConceptSchemeVersion,
	lang, descr
	from @CSList

IF @IsStub=0 BEGIN
INSERT INTO @CSXML (Tag,Parent, [ConceptScheme!2!Order!hide],[ConceptScheme!2!Order2!hide] ,
	[Concept!4!Code] )--,	[Concept!4!Type] , [Concept!4!assignmentStatus], [Concept!4!attachmentLevel]	)
	SELECT DISTINCT 4,2,ConceptSchemeID+AGENCY+ConceptSchemeVersion, ConceptCode,
	ConceptCode--, CompType, assignmentStatus, attachmentLevel
	from @CSList

INSERT INTO @CSXML (Tag,Parent, [ConceptScheme!2!Order!hide],[ConceptScheme!2!Order2!hide] ,
	[Concept!4!Code] ,--	[Concept!4!Type] , [Concept!4!assignmentStatus], [Concept!4!attachmentLevel],
	[Name!5!LocaleIsoCode],	[Name!5!!cdata]	)
	SELECT DISTINCT  5,4,ConceptSchemeID+AGENCY+ConceptSchemeVersion, ConceptCode + '_l',
	ConceptCode,-- CompType, assignmentStatus, attachmentLevel	,
	CN.lang, CN.descr
	from @CSList as CSList
	inner join @ConceptNames as CN on CSList.ConceptID= CN.ConceptID
	
END



select * from @CSXML 
order by [ConceptScheme!2!Order!hide], [Concept!4!Code], [ConceptScheme!2!Order2!hide], [Name!3!LocaleIsoCode],
 [Name!5!LocaleIsoCode]
FOR XML EXPLICIT
            ";
        }

        internal static string MSGetDSD(ref List<IParameterValue> parameter)
        {
            string dec1 = string.Format(@"
            declare @CodelistCode varchar(max)= '{0}'
	        declare @CodelistAgencyId varchar(max)= '{1}' 
	        declare @CodelistVersion varchar(max)= '{2}' 
 
	        declare @DSDCode varchar(max)= '{3}' 
	        declare @DSDAgencyId varchar(max)= '{4}' 
	        declare @DSDVersion varchar(max)= '{5}' 
    
	        declare @ConceptSchemeCode varchar(max)= '{6}' 
	        declare @ConceptSchemeAgencyId varchar(max)= '{7}'
	        declare @ConceptSchemeVersion varchar(max)= '{8}' 

            declare @DFId int={9}
	        declare @IsStub bit={10}",
          (parameter.Exists(p => p.Item == "CodelistCode") ? parameter.Find(p => p.Item == "CodelistCode").Value.ToString() : ""),
          (parameter.Exists(p => p.Item == "CodelistAgencyId") ? parameter.Find(p => p.Item == "CodelistAgencyId").Value.ToString() : ""),
          (parameter.Exists(p => p.Item == "CodelistVersion") ? parameter.Find(p => p.Item == "CodelistVersion").Value.ToString() : ""),
          (parameter.Exists(p => p.Item == "DSDCode") ? parameter.Find(p => p.Item == "DSDCode").Value.ToString() : ""),
          (parameter.Exists(p => p.Item == "DSDAgencyId") ? parameter.Find(p => p.Item == "DSDAgencyId").Value.ToString() : ""),
          (parameter.Exists(p => p.Item == "DSDVersion") ? parameter.Find(p => p.Item == "DSDVersion").Value.ToString() : ""),
          (parameter.Exists(p => p.Item == "ConceptSchemeCode") ? parameter.Find(p => p.Item == "ConceptSchemeCode").Value.ToString() : ""),
          (parameter.Exists(p => p.Item == "ConceptSchemeAgencyId") ? parameter.Find(p => p.Item == "ConceptSchemeAgencyId").Value.ToString() : ""),
          (parameter.Exists(p => p.Item == "ConceptSchemeVersion") ? parameter.Find(p => p.Item == "ConceptSchemeVersion").Value.ToString() : ""),
          (parameter.Exists(p => p.Item == "DFId") ? parameter.Find(p => p.Item == "DFId").Value.ToString() : "null"),
          (parameter.Exists(p => p.Item == "IsStub") ? parameter.Find(p => p.Item == "IsStub").Value.ToString() : "0"));
            
            return dec1 + @"

declare @DSDList table
(
	DF_ID bigint,
	DSD_ID bigint,
	DsdCode varchar(max),
	DSDAgency varchar(max), 
	DsdVersion varchar(max),
	lang varchar(max),
	descr varchar(max),
	
	COMP_ID bigint,
	conceptRef varchar(max),
	ConceptType varchar(max),
	IsFrequencyDimension int,
	ATT_STATUS varchar(max),
	ATT_ASS_LEVEL varchar(max),

	conceptSchemeRef varchar(max),
	conceptSchemeAgency varchar(max),
	conceptVersion varchar(max),

	codelist varchar(max),
	codelistAgency varchar(max),
	codelistVersion varchar(max)
)

insert into @DSDList
select DISTINCT DF_ID, DSD.DSD_ID, ARTDSD.ID as DsdCode, ARTDSD.AGENCY as DSDAgency, LTRIM(STR(ARTDSD.VERSION1)) + '.' + LTRIM(STR(ARTDSD.VERSION2)) as DsdVersion, 
LANGUAGE as lang, TEXT as descr

, COMPONENT.COMP_ID ,COMPONENT.ID as conceptRef, COMPONENT.TYPE as ConceptType,COMPONENT.IS_FREQ_DIM as IsFrequencyDimension,
COMPONENT.ATT_STATUS, COMPONENT.ATT_ASS_LEVEL,
ARTConceptScheme.ID as conceptSchemeRef,
ARTConceptScheme.AGENCY as conceptSchemeAgency,
LTRIM(STR(ARTConceptScheme.VERSION1)) + '.' + LTRIM(STR(ARTConceptScheme.VERSION2))  as conceptVersion,

ARTcodelist.ID  as codelist,
ARTcodelist.AGENCY  as codelistAgency,
LTRIM(STR(ARTcodelist.VERSION1)) + '.' + LTRIM(STR(ARTcodelist.VERSION2))  as codelistVersion


from DSD
inner join ARTEFACT as ARTDSD  on ARTDSD.ART_ID= DSD.DSD_ID
inner join LOCALISED_STRING on ARTDSD.ART_ID= LOCALISED_STRING.ART_ID
inner join COMPONENT on COMPONENT.DSD_ID=DSD.DSD_ID
inner join DATAFLOW on DATAFLOW.DSD_ID=DSD.DSD_ID

LEFT JOIN CONCEPT on CONCEPT.CON_ID= COMPONENT.CON_ID
LEFT JOIN ARTEFACT as ARTConceptScheme on ARTConceptScheme.ART_ID = CONCEPT.CON_SCH_ID
LEFT JOIN ARTEFACT as ARTcodelist on ARTcodelist.ART_ID = COMPONENT.CL_ID
--Where DSDCode
where  LOCALISED_STRING.TYPE='Name' AND
ARTDSD.ID = case when LEN(@DSDCode)>0 then @DSDCode else ARTDSD.ID end
--Agency e Version
AND ARTDSD.AGENCY = case when LEN(@DSDAgencyId)>0 then @DSDAgencyId else ARTDSD.AGENCY end
AND  LTRIM(STR(ARTDSD.VERSION1)) + '.' + LTRIM(STR(ARTDSD.VERSION2))= case when LEN(@DSDVersion)>0 then @DSDVersion else LTRIM(STR(ARTDSD.VERSION1)) + '.' + LTRIM(STR(ARTDSD.VERSION2)) end
-- ConceptScheme
AND ARTDSD.ID =
case when LEN(@ConceptSchemeCode)>0 then (Select DISTINCT ARTEFACT.ID 
FROM DSD
inner join ARTEFACT  on ARTEFACT.ART_ID= DSD.DSD_ID 
inner join COMPONENT on COMPONENT.DSD_ID=DSD.DSD_ID
LEFT JOIN CONCEPT on CONCEPT.CON_ID= COMPONENT.CON_ID
LEFT JOIN ARTEFACT as ARTConceptScheme on ARTConceptScheme.ART_ID = CONCEPT.CON_SCH_ID
where ARTConceptScheme.ID=@ConceptSchemeCode AND ARTEFACT.ID=ARTDSD.ID
AND ARTConceptScheme.AGENCY = case when LEN(@ConceptSchemeAgencyId)>0 then @ConceptSchemeAgencyId else ARTConceptScheme.AGENCY end
AND  LTRIM(STR(ARTConceptScheme.VERSION1)) + '.' + LTRIM(STR(ARTConceptScheme.VERSION2))= case when LEN(@ConceptSchemeVersion)>0 then @ConceptSchemeVersion else LTRIM(STR(ARTConceptScheme.VERSION1)) + '.' + LTRIM(STR(ARTConceptScheme.VERSION2)) end
)
else ARTDSD.ID end

-- Codelist
AND ARTDSD.ID =
case when LEN(@CodelistCode)>0 then (Select DISTINCT ARTEFACT.ID 
FROM DSD
inner join ARTEFACT  on ARTEFACT.ART_ID= DSD.DSD_ID 
inner join COMPONENT on COMPONENT.DSD_ID=DSD.DSD_ID
LEFT JOIN ARTEFACT as ARTCodelist on ARTcodelist.ART_ID = COMPONENT.CL_ID
where ARTCodelist.ID=@CodelistCode AND ARTEFACT.ID=ARTDSD.ID
AND ARTCodelist.AGENCY = case when LEN(@CodelistAgencyId)>0 then @CodelistAgencyId else ARTCodelist.AGENCY end
AND  LTRIM(STR(ARTCodelist.VERSION1)) + '.' + LTRIM(STR(ARTCodelist.VERSION2))= case when LEN(@CodelistVersion)>0 then @CodelistVersion else LTRIM(STR(ARTCodelist.VERSION1)) + '.' + LTRIM(STR(ARTCodelist.VERSION2)) end
)
  else ARTDSD.ID end
-- DF
AND DF_ID = 
case when @DFId is not null then @DFId else DF_ID end
--Ordinamento fondamentale per la Position
order by DSD.DSD_ID, COMP_ID


declare @DSDXML table
(
	Tag int,
	Parent int null,
	[DataStructures!1!xmlns] varchar(max) null,
	[DataStructure!2!Order!hide] varchar(max) null,
	[DataStructure!2!Order2!hide] varchar(max) null,
	[DataStructure!2!id] varchar(max) null,
	[DataStructure!2!agencyID] varchar(max) null,
	[DataStructure!2!version] varchar(max) null,
	[Name!3!LocaleIsoCode] char(2) null,
	[Name!3!!cdata] varchar(max) null,
	[Components!4!] varchar(max) null,
	[Components!4!Order2!hide] bigint null,
	
	[Dimension!5!conceptRef] varchar(max) null,
	[Dimension!5!codelist] varchar(max) null,
	[Dimension!5!codelistAgency] varchar(max) null,
	[Dimension!5!codelistVersion] varchar(max) null,
	[Dimension!5!conceptSchemeRef] varchar(max) null,
	[Dimension!5!conceptSchemeAgency] varchar(max) null,
	[Dimension!5!conceptVersion] varchar(max) null,
	[Dimension!5!isFrequencyDimension] varchar(max) null,
	
	[TimeDimension!6!conceptRef] varchar(max) null,
	[TimeDimension!6!codelist] varchar(max) null,
	[TimeDimension!6!codelistAgency] varchar(max) null,
	[TimeDimension!6!codelistVersion] varchar(max) null,
	[TimeDimension!6!conceptSchemeRef] varchar(max) null,
	[TimeDimension!6!conceptSchemeAgency] varchar(max) null,
	[TimeDimension!6!conceptVersion] varchar(max) null,

	[PrimaryMeasure!7!conceptRef] varchar(max) null,
	[PrimaryMeasure!7!codelist] varchar(max) null,
	[PrimaryMeasure!7!codelistAgency] varchar(max) null,
	[PrimaryMeasure!7!codelistVersion] varchar(max) null,
	[PrimaryMeasure!7!conceptSchemeRef] varchar(max) null,
	[PrimaryMeasure!7!conceptSchemeAgency] varchar(max) null,
	[PrimaryMeasure!7!conceptVersion] varchar(max) null,
	
	[Attribute!8!conceptRef] varchar(max) null,
	[Attribute!8!codelist] varchar(max) null,
	[Attribute!8!codelistAgency] varchar(max) null,
	[Attribute!8!codelistVersion] varchar(max) null,
	[Attribute!8!conceptSchemeRef] varchar(max) null,
	[Attribute!8!conceptSchemeAgency] varchar(max) null,
	[Attribute!8!conceptVersion] varchar(max) null,
	[Attribute!8!assignmentStatus] varchar(max) null,
	[Attribute!8!attachmentLevel] varchar(max) null,
	[Attribute!8!attachmentGroup] varchar(max) null,
	
	[Group!9!id] varchar(max) null,
	[GroupDimension!10!id] varchar(max) null,
	[DimensionRef!11!id] varchar(max) null


)

INSERT INTO @DSDXML (Tag,[DataStructures!1!xmlns]) Values (1,'http://istat.it/OnTheFly')

INSERT INTO @DSDXML (Tag,Parent, [DataStructure!2!Order!hide],			
	[DataStructure!2!Order2!hide]	,		
	[DataStructure!2!id],					
	[DataStructure!2!agencyID],				
	[DataStructure!2!version])  
	SELECT DISTINCT 2,1,DSD_ID, '02',
	DsdCode, DSDAgency, DsdVersion
	from @DSDList

	INSERT INTO @DSDXML (Tag,Parent, [DataStructure!2!Order!hide],			
	[DataStructure!2!Order2!hide],		
	[DataStructure!2!id],					
	[DataStructure!2!agencyID],				
	[DataStructure!2!version],[Name!3!LocaleIsoCode],					
	[Name!3!!cdata]	)  
	SELECT DISTINCT 3,2,DSD_ID, '03',
	DsdCode, DSDAgency, DsdVersion, lang, descr
	from @DSDList

IF @IsStub=0 BEGIN
	INSERT INTO @DSDXML (Tag,Parent, [DataStructure!2!Order!hide],			
	[DataStructure!2!Order2!hide]	,		
	[DataStructure!2!id],					
	[DataStructure!2!agencyID],				
	[DataStructure!2!version], [Components!4!])  
	SELECT DISTINCT 4,2,DSD_ID, '04',
	DsdCode, DSDAgency, DsdVersion,NULL
	from @DSDList


	--Dimensions
	INSERT INTO @DSDXML (Tag,Parent, [DataStructure!2!Order!hide],			
	[DataStructure!2!Order2!hide]	,		
	[Components!4!Order2!hide],

	[Dimension!5!conceptRef],				
	[Dimension!5!codelist]		,			
	[Dimension!5!codelistAgency],			
	[Dimension!5!codelistVersion],			
	[Dimension!5!conceptSchemeRef]	,		
	[Dimension!5!conceptSchemeAgency],		
	[Dimension!5!conceptVersion],
	[Dimension!5!isFrequencyDimension])  
	SELECT DISTINCT 5,4,DSD_ID, '1Dimension' + LTRIM(STR(COMP_ID)),COMP_ID,
	conceptRef, codelist, codelistAgency, codelistVersion,
	conceptSchemeRef, conceptSchemeAgency, conceptVersion,
	case when IsFrequencyDimension is null then null else 'true' end
	from @DSDList
	where ConceptType='Dimension'
	order by COMP_ID

	--TimeDimension
	INSERT INTO @DSDXML (Tag,Parent, [DataStructure!2!Order!hide],			
	[DataStructure!2!Order2!hide]	,		
	[Components!4!Order2!hide],

	[TimeDimension!6!conceptRef],				
	[TimeDimension!6!codelist]		,			
	[TimeDimension!6!codelistAgency],			
	[TimeDimension!6!codelistVersion],			
	[TimeDimension!6!conceptSchemeRef]	,		
	[TimeDimension!6!conceptSchemeAgency],		
	[TimeDimension!6!conceptVersion])  
	SELECT DISTINCT 6,4,DSD_ID, '2TimeDimension'+ LTRIM(STR(COMP_ID)),COMP_ID,
	conceptRef, codelist, codelistAgency, codelistVersion,
	conceptSchemeRef, conceptSchemeAgency, conceptVersion
	from @DSDList
	where ConceptType='TimeDimension'
	order by COMP_ID
	
	--PrimaryMeasure
	INSERT INTO @DSDXML (Tag,Parent, [DataStructure!2!Order!hide],			
	[DataStructure!2!Order2!hide]	,		
	[Components!4!Order2!hide],

	[PrimaryMeasure!7!conceptRef],				
	[PrimaryMeasure!7!codelist]		,			
	[PrimaryMeasure!7!codelistAgency],			
	[PrimaryMeasure!7!codelistVersion],			
	[PrimaryMeasure!7!conceptSchemeRef]	,		
	[PrimaryMeasure!7!conceptSchemeAgency],		
	[PrimaryMeasure!7!conceptVersion])  
	SELECT DISTINCT 7,4,DSD_ID, '3PrimaryMeasure'+ LTRIM(STR(COMP_ID)),COMP_ID,
	conceptRef, codelist, codelistAgency, codelistVersion,
	conceptSchemeRef, conceptSchemeAgency, conceptVersion
	from @DSDList
	where ConceptType='PrimaryMeasure'
	order by COMP_ID

	--Attribute
	INSERT INTO @DSDXML (Tag,Parent, [DataStructure!2!Order!hide],			
	[DataStructure!2!Order2!hide]	,		
	[Components!4!Order2!hide],

	[Attribute!8!conceptRef],				
	[Attribute!8!codelist]		,			
	[Attribute!8!codelistAgency],			
	[Attribute!8!codelistVersion],			
	[Attribute!8!conceptSchemeRef]	,		
	[Attribute!8!conceptSchemeAgency],		
	[Attribute!8!conceptVersion],
	[Attribute!8!assignmentStatus],
	[Attribute!8!attachmentLevel],
	[Attribute!8!attachmentGroup] )  
	SELECT DISTINCT 8,4,DSD_ID, '6Attribute'+ LTRIM(STR(COMP_ID)),COMP_ID,
	conceptRef, codelist, codelistAgency, codelistVersion,
	conceptSchemeRef, conceptSchemeAgency, conceptVersion,
	ATT_STATUS as assignmentStatus,
	Case ATT_ASS_LEVEL 
	When 'Series' THEN 'DimensionGroup'
	When 'Observation' THEN 'Observation'
	When 'DataSet' THEN 'Dataset'
	When 'Group' THEN 'Group'
    END	 as attachmentLevel,

	Case ATT_ASS_LEVEL 
	When 'Group' THEN 
	(Select DISTINCT DSD_GROUP.ID from ATT_GROUP inner join DSD_GROUP on DSD_GROUP.GR_ID=ATT_GROUP.GR_ID where ATT_GROUP.COMP_ID= COMP_ID)
	else Null   END	 as attachmentGroup

	from @DSDList
	where ConceptType='Attribute'
	order by COMP_ID
	

	--Groups
	INSERT INTO @DSDXML (Tag,Parent, [DataStructure!2!Order!hide],			
	[DataStructure!2!Order2!hide]	,	
	[Group!9!id])
	SELECT DISTINCT 9,4,DSD_GROUP.DSD_ID, '4Group' + 
	DSD_GROUP.ID  , DSD_GROUP.ID  
	from ATT_GROUP 
	inner join DSD_GROUP on DSD_GROUP.GR_ID=ATT_GROUP.GR_ID 
	inner join @DSDList as DSDList on DSDList.COMP_ID= ATT_GROUP.COMP_ID
	where DSDList.ATT_ASS_LEVEL='Group'

	--DimensionGroups
	INSERT INTO @DSDXML (Tag,Parent, [DataStructure!2!Order!hide],			
	[DataStructure!2!Order2!hide]	,	
	[Group!9!id], [GroupDimension!10!id])
	SELECT DISTINCT 10,9,DSD_GROUP.DSD_ID, '5Group' + 
	DSD_GROUP.ID   + DimensionGroup.conceptRef, DSD_GROUP.ID  ,DimensionGroup.conceptRef
	from ATT_GROUP 
	inner join DSD_GROUP on DSD_GROUP.GR_ID=ATT_GROUP.GR_ID 
	inner join @DSDList as DSDList on DSDList.COMP_ID= ATT_GROUP.COMP_ID
	inner join DIM_GROUP on DIM_GROUP.GR_ID=  ATT_GROUP.GR_ID
	inner join @DSDList as DimensionGroup on DimensionGroup.COMP_ID = DIM_GROUP.COMP_ID
	where DSDList.ATT_ASS_LEVEL='Group'

	INSERT INTO @DSDXML (Tag,
						Parent, 
						[DataStructure!2!Order!hide],
						[DataStructure!2!Order2!hide],
						[DimensionRef!11!id])
	SELECT 11,8,A.DSD_ID,	
			'6Attribute'+ LTRIM(STR(A.COMP_ID)) + C.ID,
			C.ID
	FROM @DSDList A
		INNER JOIN ATTR_DIMS B ON
			B.ATTR_ID = A.COMP_ID	
		INNER JOIN COMPONENT C ON
			C.COMP_ID = B.DIM_ID


END

select * from @DSDXML 
order by  [DataStructure!2!Order!hide],[DataStructure!2!Order2!hide]--,[Attribute!8!conceptRef],[Group!9!id],[GroupDimension!10!id]
FOR XML EXPLICIT
            ";
        }

        internal static string MSGetDataflows(ref List<IParameterValue> parameter)
        {
            string dec1 = string.Format(@"
            declare @DFId int={0}",
            (parameter.Exists(p => p.Item == "DFId") ? parameter.Find(p => p.Item == "DFId").Value.ToString() : "null"));
            return dec1 + @"
declare @DFList table
(
	DF_ID bigint,
	PRODUCTION int,
	DFCode varchar(max), 
	DFAgencyId varchar(max), 
	DFVersion varchar(max),

	DSDCode varchar(max), 
	DSDAgencyId varchar(max), 
	DSDVersion varchar(max),
	
	DSName varchar(max),

	DFLang char(2),
	DFName varchar(max)
)

insert into @DFList
select DISTINCT DATAFLOW.DF_ID, PRODUCTION, DF.ID, DF.AGENCY, LTRIM(STR(DF.VERSION1)) + '.' + LTRIM(STR(DF.VERSION2)),
ARTDSD.ID, ARTDSD.AGENCY, LTRIM(STR(ARTDSD.VERSION1)) + '.' + LTRIM(STR(ARTDSD.VERSION2)),
Dataset.NAME,
LOCALISED_STRING.LANGUAGE as lng, LOCALISED_STRING.TEXT as DfName
from DATAFLOW
left join ARTEFACT as DF on DF.ART_ID=DF_ID
inner join ARTEFACT as ARTDSD  on ARTDSD.ART_ID= DATAFLOW.DSD_ID
inner join LOCALISED_STRING on LOCALISED_STRING.ART_ID= DF.ART_ID
left join DATAFLOW_dataset on DATAFLOW_dataset.DF_ID=DATAFLOW.DF_ID
left join Dataset on Dataset.DS_ID = DATAFLOW_dataset.DS_ID
WHERE LOCALISED_STRING.TYPE='Name' AND DATAFLOW.DF_ID = 
case when @DFId is not null then @DFId else DATAFLOW.DF_ID end
AND (PRODUCTION=1 OR PRODUCTION=9)

declare @DFXML table
(
	Tag int,
	Parent int null,
	[Dataflows!1!xmlns] varchar(max) null,
	[Dataflow!2!id] varchar(max) null,
	[Dataflow!2!Production] varchar(max) null,
	[Dataflow!2!Code] varchar(max) null,
	[Dataflow!2!AgencyId] varchar(max) null,
	[Dataflow!2!Version] varchar(max) null,

	[Dataflow!2!DSDCode] varchar(max) null,
	[Dataflow!2!DSDAgencyId] varchar(max) null,
	[Dataflow!2!DSDVersion] varchar(max) null,
	
	[Name!3!LocaleIsoCode] char(2) null,
	[Name!3!!cdata] varchar(max) null,

	[DatasetList!4!] varchar(max) null,
	[Dataset!5!Code] varchar(max) null
)

INSERT INTO @DFXML (Tag,[Dataflows!1!xmlns]) Values (1,'http://istat.it/OnTheFly')

INSERT INTO @DFXML (Tag,Parent, [Dataflow!2!id], [Dataflow!2!Production], 
[Dataflow!2!Code] ,[Dataflow!2!AgencyId],[Dataflow!2!Version],
[Dataflow!2!DSDCode] ,[Dataflow!2!DSDAgencyId],[Dataflow!2!DSDVersion])  
	SELECT DISTINCT 2,1,DF_ID,
	Case PRODUCTION
	when 1 then 'MA'
	when 9 then 'DL'
	end,
	DFCode,	DFAgencyId ,DFVersion,
	DSDCode,	DSDAgencyId ,DSDVersion
	from @DFList

	INSERT INTO @DFXML (Tag,Parent, [Dataflow!2!id], [Dataflow!2!Production], 
[Dataflow!2!Code] ,[Dataflow!2!AgencyId],[Dataflow!2!Version],
[Dataflow!2!DSDCode] ,[Dataflow!2!DSDAgencyId],[Dataflow!2!DSDVersion],
[Name!3!LocaleIsoCode], [Name!3!!cdata] )  
	SELECT DISTINCT 3,2,DF_ID,
	Case PRODUCTION
	when 1 then 'MA'
	when 9 then 'DL'
	end,
	DFCode,	DFAgencyId ,DFVersion,
	DSDCode,	DSDAgencyId ,DSDVersion, 
	DFLang, DFName
	from @DFList

	INSERT INTO @DFXML (Tag,Parent, [Dataflow!2!id], [DatasetList!4!] )  
	SELECT DISTINCT 4,2,DF_ID,Null
	from @DFList Where DSName is not NULL

	INSERT INTO @DFXML (Tag,Parent, [Dataflow!2!id], [Dataset!5!Code] )  
	SELECT DISTINCT 5,4,DF_ID,
	DSName
	from @DFList Where DSName is not NULL


select * from @DFXML order by [Dataflow!2!id], Tag
FOR XML EXPLICIT
            ";
        }

        internal static string MSGetCategoryAndCategorisation(ref List<IParameterValue> parameter)
        {
            return @"

create table  #Themes 
(   
	IDCategoryScheme int,
	IDCategory int,
	IDCategoryParent int,
	DimMemberIsoCode varchar(max),
	DimMemberDescription varchar(max),
	DFId int,
	Livel int,
	Ordinamento varchar(max)

)
Declare    @ThemeChild Table
(   
	IDCategoryScheme int,
	IDCategory int,
	IDCategoryParent int,
	DimMemberIsoCode varchar(max),
	DimMemberDescription varchar(max),
	DFId int,
	Livel int,
	Ordinamento varchar(max)
)


Insert into @ThemeChild
select 
	CATEGORY.CAT_SCH_ID,
	CATEGORY.CAT_ID,
	CATEGORY.PARENT_CAT_ID,
	ItemName.LANGUAGE as CategoryLang, 
	ItemName.TEXT as CategoryName,
    CATEGORISATION.ART_ID,
	1,
	cast(CATEGORY.CAT_ID as varchar(max))
 FROM CATEGORY
  inner join LOCALISED_STRING as ItemName on ItemName.ITEM_ID=CAT_ID
  left join CATEGORISATION on CATEGORISATION.CAT_ID=CATEGORY.CAT_ID 
where ItemName.TYPE='Name' 
AND CATEGORY.PARENT_CAT_ID is NULL



Declare @FigliCount int
SET @FigliCount = (select count(IDCategory) from @ThemeChild)
Declare @Livel int = 2
WHILE @FigliCount>0
BEGIN
	insert into #Themes
	select * from @ThemeChild

	delete from @ThemeChild

	Insert into @ThemeChild
	select 
		CATEGORY.CAT_SCH_ID,
		CATEGORY.CAT_ID,
		CATEGORY.PARENT_CAT_ID,
		ItemName.LANGUAGE as CategoryLang, 
		ItemName.TEXT as CategoryName,
		CATEGORISATION.ART_ID,
		@Livel,
		(Select top 1 Ordinamento  from #Themes as O where O.IDCategory=CATEGORY.PARENT_CAT_ID AND Livel=@Livel-1) + '-' + cast(CATEGORY.CAT_ID as varchar(max))
		FROM CATEGORY
		inner join LOCALISED_STRING as ItemName on ItemName.ITEM_ID=CAT_ID
		left join CATEGORISATION on CATEGORISATION.CAT_ID=CATEGORY.CAT_ID 
		where ItemName.TYPE='Name' 
		AND CATEGORY.PARENT_CAT_ID in (Select IDCategory from #Themes where Livel=@Livel-1) 
		AND CATEGORY.PARENT_CAT_ID is not NULL

	Set @Livel=@Livel+1
	SET @FigliCount = (select count(IDCategory) from @ThemeChild)
END


--exec('drop table #T')
create table #T
(
	Tag int,
	Parent int null,
	[ThemeList!1!xmlns] varchar(max) null,
	[ThemeList!1!Order!hide] varchar(max) null,
	[CategoryScheme!2!Code] varchar(max) null,
	[CategoryScheme!2!Order!hide] varchar(max) null,
	[CategoryScheme!2!AgencyId] varchar(max) null,
	[CategoryScheme!2!Version] varchar(max) null,
	[Name!3!LocaleIsoCode] char(2) null,
	[Name!3!!cdata]varchar(max) null,
	[ContentObject!4!type] varchar(max) null,
	[ContentObject!4!Code] varchar(max) null,
	[Name!5!LocaleIsoCode] char(2) null,
	[Name!5!!cdata]varchar(max) null,
	[DataflowList!6!] varchar(max) null,
	[Dataflow!7!Code] varchar(max) null,
	[Name!8!LocaleIsoCode] char(2) null,
	[Name!8!!cdata] varchar(max) null
)
declare @i int
declare @si0 varchar(max)
declare @si1 varchar(max)
declare @si2 varchar(max)
declare @si3 varchar(max)
declare @si4 varchar(max)
declare @si5 varchar(max)
declare @siParent varchar(max)
declare @execstr varchar(max)
declare @maxdepth int;
set @maxdepth =(select MAX(Livel) from #Themes)
set @execstr = ''
set @i = 2
while (@i <= @maxdepth)
begin
	set @si1 = cast(5*@i-1 as varchar(max))
	set @si2 = cast(5*@i as varchar(max))
	set @si3 = cast(5*@i+1 as varchar(max))
	set @si4 = cast(5*@i+2 as varchar(max))
	set @si5 = cast(5*@i+3 as varchar(max))
	if (len(@execstr) > 0)
		set @execstr = @execstr + ','
	set @execstr = @execstr + '
	[ContentObject!'+@si1+'!type] varchar(max) null,
	[ContentObject!'+@si1+'!Code] varchar(max) null,
	[Name!'+@si2+'!LocaleIsoCode] char(2) null,
	[Name!'+@si2+'!!cdata]varchar(max) null,
	[DataflowList!'+@si3+'!] varchar(max) null,
	[Dataflow!'+@si4+'!Code] varchar(max) null,
	[Name!'+@si5+'!LocaleIsoCode] char(2) null,
	[Name!'+@si5+'!!cdata] varchar(max) null'
	set @i = @i+1
end

if (len(@execstr) > 0)
	exec('alter table #T add '+@execstr)

INSERT INTO #T (Tag,[ThemeList!1!xmlns]) Values (1,'http://istat.it/OnTheFly')

EXEC('INSERT INTO #T (Tag,Parent, [CategoryScheme!2!Order!hide],
    [CategoryScheme!2!Code] ,
	[CategoryScheme!2!AgencyId],
	[CategoryScheme!2!Version] )
	select DISTINCT 2,1, IDCategoryScheme, ArtCatScheme.ID, ArtCatScheme.AGENCY,  LTRIM(STR(ArtCatScheme.VERSION1)) + ''.'' + LTRIM(STR(ArtCatScheme.VERSION2))
	from  #Themes
	Inner join ARTEFACT as ArtCatScheme on ArtCatScheme.ART_ID=IDCategoryScheme')

EXEC('INSERT INTO #T (Tag,Parent, [CategoryScheme!2!Order!hide],
    [Name!3!LocaleIsoCode],	[Name!3!!cdata])
	select DISTINCT 3,2, IDCategoryScheme, LS.LANGUAGE as lng, LS.TEXT as CSName
	from  #Themes
	inner join LOCALISED_STRING as LS on LS.ART_ID = IDCategoryScheme
	WHERE LS.TYPE=''Name''')


set @i = 1
while (@i <= @maxdepth)
begin
	set @si1 = cast(5*@i-1 as varchar(max))
	set @si2 = cast(5*@i as varchar(max))
	set @si3 = cast(5*@i+1 as varchar(max))
	set @si4 = cast(5*@i+2 as varchar(max))
	set @si5 = cast(5*@i+3 as varchar(max))
	set @siParent= 
	CASE  WHEN @i = 1 then  cast(2 as varchar(max))
	ELSE  cast(5*(@i-1)-1 as varchar(max))
	END

	EXEC('INSERT INTO #T (Tag,Parent,[CategoryScheme!2!Order!hide],[ThemeList!1!Order!hide],
	[ContentObject!'+@si1+'!type],[ContentObject!'+@si1+'!Code]) 
	select DISTINCT '+@si1+' as Tag, '+@siParent+' as Parent,IDCategoryScheme, Ordinamento, 
	''ContentTheme'', IDCategory
	from #Themes where Livel='+@i)

	EXEC('INSERT INTO #T (Tag,Parent,[CategoryScheme!2!Order!hide], [ThemeList!1!Order!hide],
	[ContentObject!'+@si1+'!type],[ContentObject!'+@si1+'!Code],
	[Name!'+@si2+'!LocaleIsoCode] ,	[Name!'+@si2+'!!cdata]) 
	select DISTINCT '+@si2+' as Tag, '+@si1+' as Parent,IDCategoryScheme, Ordinamento, 
	''ContentTheme'', IDCategory,DimMemberIsoCode, DimMemberDescription
	from #Themes where Livel='+@i)

	EXEC('INSERT INTO #T (Tag,Parent,[CategoryScheme!2!Order!hide], [ThemeList!1!Order!hide],
	[ContentObject!'+@si1+'!type],[ContentObject!'+@si1+'!Code],
	[DataflowList!'+@si3+'!]) 
	select DISTINCT '+@si3+' as Tag, '+@si1+' as Parent,IDCategoryScheme, Ordinamento, 
	''ContentTheme'', IDCategory,null
	from #Themes where Livel='+@i)

	EXEC('INSERT INTO #T (Tag,Parent,[CategoryScheme!2!Order!hide], [ThemeList!1!Order!hide],
	[ContentObject!'+@si1+'!type],[ContentObject!'+@si1+'!Code],
	[DataflowList!'+@si3+'!],[Dataflow!'+@si4+'!Code]) 
	select DISTINCT '+@si4+' as Tag, '+@si3+' as Parent,IDCategoryScheme, Ordinamento, 
	''ContentTheme'', IDCategory, NULL , DFId
	from #Themes where DFId IS NOT NULL AND Livel='+@i )

	set @i = @i+1
end

select * from #T order by [CategoryScheme!2!Order!hide],[ThemeList!1!Order!hide], [Tag]
FOR XML EXPLICIT
            ";
        }

        internal static string MSGetCategoryScheme(ref List<IParameterValue> parameter)
        {
            return "SELECT DISTINCT A.ART_ID,A.ID,A.AGENCY," +
                    "'VERSION' = CAST(A.VERSION1 AS VARCHAR(2)) +'.'+ CAST(A.VERSION2 AS VARCHAR(2)),A.IS_FINAL,C.LANGUAGE,C.TEXT " +
                    "FROM ARTEFACT A  " +
                    "    INNER JOIN CATEGORY_SCHEME B ON " +
                    "        B.CAT_SCH_ID = A.ART_ID " +
                    "    INNER JOIN LOCALISED_STRING C ON " +
                    "        C.ART_ID = A.ART_ID " +
                    " WHERE TYPE ='Name' ";
        }
    }
}
