using System;
using System.Collections;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
using System.Web.UI.WebControls;

using Navitaire.Reporting.Services.Exceptions;
using Navitaire.Reporting.Tools.FileCompression.Zip;

namespace Navitaire.Reporting.Services
{
    public class Utility
    {
        /// <summary>
        /// Compress the specified byte[] and return the compressed byte[]
        /// </summary>
        /// <param name="uncompressedBytes"></param>
        /// <param name="entryName"></param>
        /// <returns></returns>
        public static byte[] CompressStream( byte[] uncompressedValue, string entryExtension )
        {
            byte[] compressedBytes;
            using ( MemoryStream compressedStream = new MemoryStream() )
            {
                // Compress the raw bytes into the compressed stream
                using ( ZipOutputStream outStream = new ZipOutputStream( compressedStream ) )
                {
                    outStream.SetLevel( 9 ); // 0 - store only to 9 - means best compression

                    ZipEntry entry = new ZipEntry( string.Concat( "report.", entryExtension ) );
                    outStream.PutNextEntry( entry );
                    outStream.Write( uncompressedValue, 0, uncompressedValue.Length );
                    outStream.Flush();
                    outStream.Finish();

                    // Check the length for this ZipOutputStream
                    long streamLength = outStream.Length;

                    // read the compressed stream's buffer
                    compressedBytes = new byte[ streamLength ];
                    compressedStream.Seek( 0, SeekOrigin.Begin );
                    // Note: There is potential data loss here.  If the compressed stream length 
                    //       is > 2Gig, we will have an invalid length, and the file will be 
                    //       corrupt.  Rather than adding complexity for that corner case, I'm 
                    //       just noting the possibility, and using the cast.
                    compressedStream.Read( compressedBytes, 0, (int)streamLength );
                }
            }

            return compressedBytes;
        }

        public static string DecryptValue( string encryptedValue )
        {
            return Security.SecurityManager.DecryptValue( encryptedValue );
        }

        public static string EncryptValue( string decryptedValue )
        {
            return Security.SecurityManager.EncryptValue( decryptedValue );
        }

        public static void SendEmail( string smtpServer,
            StringCollection toAddressList,
            StringCollection ccAddressList,
            string bccAddress,
            string fromAddress,
            string messageSubject,
            string messageBody,
            bool isBodyHtml,
            string mailTemplate,
            Hashtable mailTemplateReplacements,
            Attachment[] attachmentList )
        {
            string adjustedMessageBody = messageBody;

            if ( ( isBodyHtml ) &&
                ( !string.IsNullOrEmpty( mailTemplate ) ) )
            {
                string templateFilePath = string.Format(
                    CultureInfo.InvariantCulture,
                    @"{0}EmailTemplates\{1}.htm",
                    AppDomain.CurrentDomain.BaseDirectory,
                    mailTemplate );
                if ( !File.Exists( templateFilePath ) )
                {
                    // Try the bin directory
                    templateFilePath = string.Format(
                        CultureInfo.InvariantCulture,
                        @"{0}bin\EmailTemplates\{1}.htm",
                        AppDomain.CurrentDomain.BaseDirectory,
                        mailTemplate );
                }

                byte[] fileContents = null;
                using ( FileStream fsTemplate = new FileStream( templateFilePath, FileMode.Open, FileAccess.Read, FileShare.Read ) )
                {
                    fileContents = new byte[ fsTemplate.Length ];
                    fsTemplate.Read( fileContents, 0, (int)fsTemplate.Length );
                }

                StringBuilder messageBodyBuilder = new StringBuilder();
                messageBodyBuilder.Append( Encoding.Default.GetChars( fileContents ) );

                // work thru all the replacements
                foreach ( string strKey in mailTemplateReplacements.Keys )
                {
                    messageBodyBuilder.Replace( strKey, (string)mailTemplateReplacements[ strKey ] );
                }

                //Encapsulate the report output to the email body
                if (adjustedMessageBody != string.Empty)
                {
                    if (templateFilePath.Contains("EmailBody"))
                    {
                        StringBuilder adjustedStringBuilder = new StringBuilder();
                        string strEmailTemplate = messageBodyBuilder.ToString();

                        //Manage the body
                        int iStart = strEmailTemplate.IndexOf("<div>", StringComparison.OrdinalIgnoreCase);
                        strEmailTemplate = strEmailTemplate.Substring(iStart);

                        int iEnd = strEmailTemplate.IndexOf("</div>", StringComparison.OrdinalIgnoreCase);
                        strEmailTemplate = strEmailTemplate.Remove(iEnd + 7);

                        iStart = adjustedMessageBody.IndexOf("<DIV", StringComparison.OrdinalIgnoreCase);

                        string strFirstHalf = adjustedMessageBody.Substring(0, iStart);
                        string strSecondHalf = adjustedMessageBody.Substring(iStart);

                        adjustedStringBuilder.Append(strFirstHalf);
                        adjustedStringBuilder.Append(strEmailTemplate);
                        adjustedStringBuilder.Append(strSecondHalf);

                        messageBodyBuilder = adjustedStringBuilder;
                    }
                }
                adjustedMessageBody = messageBodyBuilder.ToString();
            }
            SendEmail( smtpServer, toAddressList, ccAddressList, bccAddress, fromAddress, messageSubject, adjustedMessageBody, isBodyHtml, attachmentList);
        }

        public static void SendEmail( string smtpServer,
            StringCollection toAddressList,
            StringCollection ccAddressList,
            string bccAddress,
            string fromAddress,
            string messageSubject,
            string messageBody,
            bool isBodyHtml,
            Attachment[] attachmentList)
        {
            #region ValidateParameters

            if ( string.IsNullOrEmpty( smtpServer ) )
            {
                throw new ArgumentNullException( "smtpServer" );
            }
            else if ( toAddressList == null )
            {
                throw new ArgumentNullException( "toAddressList" );
            }
            else if ( string.IsNullOrEmpty( fromAddress ) )
            {
                throw new ArgumentNullException( "fromAddress" );
            }
            else if ( string.IsNullOrEmpty( messageSubject ) )
            {
                throw new ArgumentNullException( "messageSubject" );
            }
            else if ( string.IsNullOrEmpty( messageBody ) )
            {
                throw new ArgumentNullException( "messageBody" );
            }

            if ( toAddressList.Count == 0 )
            {
                throw new ArgumentException( "At least one e-mail address is expected" );
            }

            #endregion ValidateParameters

            using ( MailMessage mailMessage = new MailMessage() )
            {
                mailMessage.IsBodyHtml = isBodyHtml;

                SmtpClient smtpClient = new SmtpClient( smtpServer );

                // add any attachments
                if ( attachmentList != null )
                {
                    foreach ( Attachment attachment in attachmentList )
                    {
                        mailMessage.Attachments.Add( attachment );
                    }
                }

                // To addresses
                for ( int t = 0; t < toAddressList.Count; t++ )
                {
                    mailMessage.To.Add( new MailAddress( toAddressList[ t ] ) );
                }

                // Cc Addresses
                if ( ccAddressList != null )
                {
                    for ( int t = 0; t < ccAddressList.Count; t++ )
                    {
                        mailMessage.CC.Add( new MailAddress( ccAddressList[ t ] ) );
                    }
                }

                // Bcc Addressess
                if (!bccAddress.Equals(string.Empty))
                {
                    mailMessage.Bcc.Add(new MailAddress(bccAddress));
                }

                // From address
                mailMessage.From = new MailAddress( fromAddress );

                // Subject and body
                mailMessage.Subject = messageSubject;
                mailMessage.Body = messageBody;

                // Send the mail
                smtpClient.Send( mailMessage );
            }
        }

        /// <summary>
        /// Make sure the specified folder exists
        /// </summary>
        /// <param name="ftpFolderPath"></param>
        /// <param name="ftpNetworkCredential"></param>
        public static void FtpEnsureFolderPathExists( string ftpFolderPath, ICredentials ftpNetworkCredential )
        {
            // Make sure directory exists
            // HACK -- FTP doesn't provide a way to check for the existance of a Directory.  So 
            //         try to create the directory all the time, and catch the resulting exception 
            //         that occurs if the directory already exists.  If there was a problem 
            //         creating the directory, the problem will be exposed during the subsequent 
            //         File Upload call.
            FtpWebRequest ftpDirectoryWebRequest = WebRequest.Create( ftpFolderPath ) as FtpWebRequest;

            if ( ftpDirectoryWebRequest == null )
            {
                throw new InvalidValueException( string.Format( CultureInfo.InvariantCulture, "Unable to create an FtpWebRequest for '{0}'", ftpFolderPath ) );
            }

            ftpDirectoryWebRequest.Method = WebRequestMethods.Ftp.MakeDirectory;

            // Set credentials for Logging.
            ftpDirectoryWebRequest.Credentials = ftpNetworkCredential;

            try
            {
                //FtpWebResponse response = ftpDirectoryWebRequest.GetResponse() as FtpWebResponse;
                ftpDirectoryWebRequest.GetResponse();
            }
            catch
            {
                // Do nothing, if the folder already exists an exception will be raised.
            }
        }

        public static void FtpUploadFile( string ftpUploadFileAddress, ICredentials ftpNetworkCredential, byte[] reportArray )
        {
            FtpWebRequest ftpUploadWebRequest = WebRequest.Create( ftpUploadFileAddress ) as FtpWebRequest;

            if ( ftpUploadWebRequest == null )
            {
                throw new InvalidValueException( string.Format( CultureInfo.InvariantCulture, 
                    "Unable to create an FtpWebRequest for '{0}'", 
                    ftpUploadFileAddress ) );
            }

            ftpUploadWebRequest.Method = WebRequestMethods.Ftp.UploadFile;

            // Set credentials for Logging.
            ftpUploadWebRequest.Credentials = ftpNetworkCredential;

            // Get the stream associated with the WebRequest
            using ( Stream requestStream = ftpUploadWebRequest.GetRequestStream() )
            {
                // Write the contents to the request stream.
                requestStream.Write( reportArray, 0, reportArray.Length );

                // Close the request stream before sending the request.
                requestStream.Close();
            }
        }

        /// <summary>
        /// This is a diagnostic helper method.  If the user has enabled trace messages in the 
        /// app.config, values will be written to the console, otherwise no action is performed
        /// </summary>
        /// <remarks>
        /// The config is retrieved when the service is started.  If a user wants to change the 
        /// value, they have to stop/start the service after making the change.
        /// </remarks>
        /// <param name="message"></param>
        /// <param name="actualLogLevel"></param>
        /// <param name="requiredLogLevel"></param>
        public static void WriteTraceMessage( string message, TraceLevel actualLogLevel, TraceLevel requiredLogLevel )
        {
            bool writeTraceMessage = false;

            if ( actualLogLevel == TraceLevel.Standard )
            {
                if ( requiredLogLevel == TraceLevel.Standard )
                {
                    writeTraceMessage = true;
                }
            }
            else if ( actualLogLevel == TraceLevel.Verbose )
            {
                if ( ( requiredLogLevel == TraceLevel.Standard ) || ( requiredLogLevel == TraceLevel.Verbose ) )
                {
                    writeTraceMessage = true;
                }
            }

            if ( writeTraceMessage )
            {
                Trace.WriteLine( message );
            }
        }
    }
}

