using Abp.Auditing;
using HIPMS.Controllers;
using ITfoxtec.Identity.Saml2;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Data.SqlTypes;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace HIPMS.Web.Controllers
{
    [AllowAnonymous]
    [Route("SAMLAuth")]
    public class AuthController :  HIPMSControllerBase
    {
        const string relayStateReturnUrl = "ReturnUrl";
        private readonly Saml2Configuration config;

        public AuthController(IOptions<Saml2Configuration> configAccessor)
        {
            config = configAccessor.Value;
            Console.WriteLine("Issuer" + config.Issuer);
           

            // Your above payload.
            //string examplePayload = System.IO.File.ReadAllText("TextFile1.txt");

            //// Creating the saml response.
            //SamlResponse samlResponse = new SamlResponse(examplePayload);

            //// writing the output, rudresh@google.com
            //Console.WriteLine(samlResponse.EmailAddress);
            Console.WriteLine("SAML testing");
            Redirect("https://translate.google.com/?hl=hi&sl=auto&tl=en&op=translate");
            //Redirect("https://translate.google.com/?hl=hi&sl=auto&tl=en&op=translate");
        }
        public IActionResult Index()
        {
            return View();
        }
        
    }
    public class SamlResponse
    {
        public string EmailAddress { get; }
        public string Email { get; }

        public SamlResponse(string saml2)
        {
            if (string.IsNullOrEmpty(saml2)) throw new ArgumentNullException(nameof(saml2));

            using (MemoryStream ms = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(saml2)))
            {
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(AttributeStatement));
                AttributeStatement response = (AttributeStatement)xmlSerializer.Deserialize(ms);
                EmailAddress = response.Attribute.FirstOrDefault(x => x.Name == "emailaddress").AttributeValue?.TrimEnd();
                Email = response.Attribute.FirstOrDefault(x => x.Name == "User.email").AttributeValue?.TrimEnd();
            }
        }

        /// <remarks/>
        [Serializable()]
        [System.ComponentModel.DesignerCategory("code")]
        [XmlType(AnonymousType = true, Namespace = "urn:oasis:names:tc:SAML:2.0:assertion")]
        [XmlRoot(Namespace = "urn:oasis:names:tc:SAML:2.0:assertion", IsNullable = false)]
        public partial class AttributeStatement
        {

            private AttributeStatementAttribute[] attributeField;

            /// <remarks/>
            [XmlElement("Attribute")]
            public AttributeStatementAttribute[] Attribute
            {
                get
                {
                    return this.attributeField;
                }
                set
                {
                    this.attributeField = value;
                }
            }
        }

        /// <remarks/>
        [Serializable()]
        [System.ComponentModel.DesignerCategory("code")]
        [XmlType(AnonymousType = true, Namespace = "urn:oasis:names:tc:SAML:2.0:assertion")]
        public partial class AttributeStatementAttribute
        {
            private string attributeValueField;

            private string nameField;

            private string nameFormatField;

            /// <remarks/>
            public string AttributeValue
            {
                get
                {
                    return this.attributeValueField;
                }
                set
                {
                    this.attributeValueField = value;
                }
            }

            /// <remarks/>
            [XmlAttribute()]
            public string Name
            {
                get
                {
                    return this.nameField;
                }
                set
                {
                    this.nameField = value;
                }
            }

            /// <remarks/>
            [XmlAttribute()]
            public string NameFormat
            {
                get
                {
                    return this.nameFormatField;
                }
                set
                {
                    this.nameFormatField = value;
                }
            }
        }
    }

}
