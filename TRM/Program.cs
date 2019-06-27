using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using TRM.WSTRM;
namespace TRM
{
    class Program
    {
        static void Main(string[] args)
        {

            SAWSessionServiceSoapClient sawSession = new SAWSessionServiceSoapClient();
            string sessionID = sawSession.logon("publico", "publico");

            //Set report parameters
            ReportRef repRef = new ReportRef();
            //path: get from report http url
            repRef.reportPath = @"/shared/Consulta Series Estadisticas desde Excel/1. Tasa de Cambio Peso Colombiano"
             + "/1.1 TRM - Disponible desde el 27 de noviembre de 1991/TRM para un dia";
            //xml: get from report http url + &format=xml
            repRef.reportXml = @"<xsd:schema targetnamespace='\' urn:schemas-microsoft-com:xml-analysis:rowset=""><xsd:complextype name='\'>"
             + @"<xsd:sequence><xsd:element 35="" as="" ate="" char="" character="" dd="" de="" echa="" evaluate="" fmday="" maxoccurs='\' minoccurs='\' month="" name='\' nls_date_language='SPANISH' nonagg="" none="" saw-sql:aggregationrule='\' saw-sql:aggregationtype='\' saw-sql:columnheading='\' saw-sql:displayformula='\' saw-sql:tableheading='\' saw-sql:type='\' type='\' xsd:string="" yyyy="">"
             + @"<xsd:element a="" del="" echa="" maxoccurs='\' mes="" minoccurs='\' name='\' nonagg="" none="" saw-sql:aggregationrule='\' saw-sql:aggregationtype='\' saw-sql:columnheading='\' saw-sql:displayformula='\' saw-sql:tableheading='\' saw-sql:type='\' tinyint="" type='\' xsd:byte="">"
             + @"<xsd:element agg="" double="" max="" maxoccurs='\' minoccurs='\' name='\' saw-sql:aggregationrule='\' saw-sql:aggregationtype='\' saw-sql:columnheading='\' saw-sql:displayformula='\' saw-sql:tableheading='\' saw-sql:type='\' type='\' xsd:double=""></xsd:element></xsd:element></xsd:element></xsd:sequence></xsd:complextype></xsd:schema>";

            //Create xml view, set xml options
            XmlViewServiceSoapClient xmlView = new XmlViewServiceSoapClient();
            XMLQueryExecutionOptions xmlOpts = new XMLQueryExecutionOptions();
            xmlOpts.maxRowsPerPage = 100;
            xmlOpts.refresh = true; 


            //Pass report parameters
            ReportParams repParams = new ReportParams();

            //Execute XML Query
            QueryResults qResults = xmlView.executeXMLQuery(repRef, XMLQueryOutputFormat.SAWRowsetData, xmlOpts, repParams, sessionID);

            //Print rowset
            sawSession.logoff(sessionID);

            //Get rate value from result XML
            XmlDocument results = new XmlDocument();
            results.LoadXml(qResults.rowset);

            DateTime trmDate = DateTime.Today.AddDays(-5);
            XmlNodeList xnlDate = results.GetElementsByTagName("Column0");
            foreach (XmlNode xn in xnlDate)
                trmDate = DateTime.ParseExact(xn.InnerText, "dddd d 'de' MMMM 'de' yyyy", CultureInfo.CreateSpecificCulture("es-CO"));

            decimal trm = decimal.Zero;
            XmlNodeList xnlRate = results.GetElementsByTagName("Column2");
            foreach (XmlNode xn in xnlRate)
                trm = Convert.ToDecimal(xn.InnerText, CultureInfo.InvariantCulture);


            Console.Write(trm);
            Console.Read();

        }
    }
}
