// Copyright PHOENIX CONTACT Electronics GmbH

/* ========================================================================
 * Copyright (c) 2005-2019 The OPC Foundation, Inc. All rights reserved.
 *
 * OPC Foundation MIT License 1.00
 * 
 * Permission is hereby granted, free of charge, to any person
 * obtaining a copy of this software and associated documentation
 * files (the "Software"), to deal in the Software without
 * restriction, including without limitation the rights to use,
 * copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the
 * Software is furnished to do so, subject to the following
 * conditions:
 * 
 * The above copyright notice and this permission notice shall be
 * included in all copies or substantial portions of the Software.
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
 * EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
 * OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
 * NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
 * HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
 * WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
 * FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
 * OTHER DEALINGS IN THE SOFTWARE.
 *
 * The complete license agreement can be found here:
 * http://opcfoundation.org/License/MIT/1.00/
 * ======================================================================*/

using Opc.Ua;
using Opc.Ua.Client;
using Opc.Ua.Configuration;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Arp.OpcUA.Client
{
    public class OpcClient : IDisposable
    {
        private const int ReconnectPeriod = 10;
        private ISession session;
        private SessionReconnectHandler reconnectHandler;
        private readonly string endpointURL;
        private readonly int clientRunTime = Timeout.Infinite;
        private static bool autoAccept;
        private readonly MessageSecurityMode securityMode;
        private readonly string securityPolicyUri;
        private readonly string userName;
        private readonly string password;

        public OpcClient(string endpointURL, bool autoAccept, int stopTimeout, MessageSecurityMode securityMode, string securityPolicyUri, string userName = "", string password = "")
        {
            this.endpointURL = endpointURL;
            OpcClient.autoAccept = autoAccept;
            this.securityMode = securityMode;
            this.securityPolicyUri = securityPolicyUri;
            clientRunTime = stopTimeout <= 0 ? Timeout.Infinite : stopTimeout * 1000;
            this.userName = userName;
            this.password = password;
        }

        public async Task<ISession> Connect()
        {
            UserIdentity identity = null;
            if (string.IsNullOrWhiteSpace(userName))
            {
                identity = new UserIdentity(new AnonymousIdentityToken());
            }
            else
            {
                identity = new UserIdentity(userName, password);
            }

            Console.WriteLine("1 - Create an Application Configuration.");

            ApplicationInstance application = new ApplicationInstance
            {
                ApplicationType = ApplicationType.Client,
                ConfigSectionName = "Opc.Ua.Client"
            };

            // load the application configuration.
            ApplicationConfiguration config = await application.LoadApplicationConfiguration(false);

            // check the application certificate.
            bool haveAppCertificate = await application.CheckApplicationInstanceCertificate(false, 0);
            if (!haveAppCertificate)
            {
                throw new Exception("Application instance certificate invalid!");
            }

            if (haveAppCertificate)
            {
                config.ApplicationUri = X509Utils.GetApplicationUriFromCertificate(config.SecurityConfiguration.ApplicationCertificate.Certificate);
                if (config.SecurityConfiguration.AutoAcceptUntrustedCertificates)
                {
                    autoAccept = true;
                }
                config.CertificateValidator.CertificateValidation += new CertificateValidationEventHandler(CertificateValidator_CertificateValidation);
            }
            else
            {
                Console.WriteLine("    WARN: missing application certificate, using unsecure connection.");
            }

            Console.WriteLine("2 - Discover endpoints of {0}.", endpointURL);
            EndpointDescription selectedEndpoint = null;
            try
            {
                selectedEndpoint = CoreClientUtils.SelectEndpoint(endpointURL, haveAppCertificate, 15000);
                session = await CreateSession(config, selectedEndpoint, identity);
            }
            catch { }
            if (session == null)
            {
                selectedEndpoint = CoreClientUtils.SelectEndpoint(endpointURL, false, 15000);
                session = await CreateSession(config, selectedEndpoint, identity);
            }
            // register keep alive handler
            session.KeepAlive += Client_KeepAlive;

            return session;
        }

        private async Task<Session> CreateSession(ApplicationConfiguration config, EndpointDescription selectedEndpoint, UserIdentity identity)
        {
            Console.WriteLine("    Selected endpoint uses: {0}",
                selectedEndpoint.SecurityPolicyUri.Substring(selectedEndpoint.SecurityPolicyUri.LastIndexOf('#') + 1));

            Console.WriteLine("3 - Create a session with OPC UA server.");
            EndpointConfiguration endpointConfiguration = EndpointConfiguration.Create(config);
            ConfiguredEndpoint endpoint = new ConfiguredEndpoint(null, selectedEndpoint, endpointConfiguration);

            return await Session.Create(config, endpoint, false, "OPC UA Console Client", 60000, identity, null);
        }

        private void Client_KeepAlive(ISession sender, KeepAliveEventArgs e)
        {
            if (e.Status != null && ServiceResult.IsNotGood(e.Status))
            {
                Console.WriteLine("{0} {1}/{2}", e.Status, sender.OutstandingRequestCount, sender.DefunctRequestCount);

                if (reconnectHandler == null)
                {
                    Console.WriteLine("--- RECONNECTING ---");
                    reconnectHandler = new SessionReconnectHandler();
                    reconnectHandler.BeginReconnect(sender, ReconnectPeriod * 1000, Client_ReconnectComplete);
                }
            }
        }

        private void Client_ReconnectComplete(object sender, EventArgs e)
        {
            // ignore callbacks from discarded objects.
            if (!ReferenceEquals(sender, reconnectHandler))
            {
                return;
            }

            session = reconnectHandler.Session;
            reconnectHandler.Dispose();
            reconnectHandler = null;

            Console.WriteLine("--- RECONNECTED ---");
        }

        private static void CertificateValidator_CertificateValidation(CertificateValidator validator, CertificateValidationEventArgs e)
        {
            if (e.Error.StatusCode == StatusCodes.BadCertificateUntrusted)
            {
                e.Accept = autoAccept;
                if (autoAccept)
                {
                    Console.WriteLine("Accepted Certificate: {0}", e.Certificate.Subject);
                }
                else
                {
                    Console.WriteLine("Rejected Certificate: {0}", e.Certificate.Subject);
                }
            }
        }
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                reconnectHandler?.Dispose();
            }
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
