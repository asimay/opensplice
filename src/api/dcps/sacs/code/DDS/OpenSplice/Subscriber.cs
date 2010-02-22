﻿// The OpenSplice DDS Community Edition project.
//
// Copyright (C) 2006 to 2009 PrismTech Limited and its licensees.
// Copyright (C) 2009  L-3 Communications / IS
// 
//  This library is free software; you can redistribute it and/or
//  modify it under the terms of the GNU Lesser General Public
//  License Version 3 dated 29 June 2007, as published by the
//  Free Software Foundation.
// 
//  This library is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
//  Lesser General Public License for more details.
// 
//  You should have received a copy of the GNU Lesser General Public
//  License along with OpenSplice DDS Community Edition; if not, write to the Free Software
//  Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301  USA

using System;
using DDS;
using DDS.OpenSplice.CustomMarshalers;

namespace DDS.OpenSplice
{
    internal class Subscriber : Entity, ISubscriber
    {
        private readonly SubscriberListenerHelper listenerHelper;

        internal Subscriber(IntPtr gapiPtr)
            : base(gapiPtr)
        {
            // Base class handles everything.
            listenerHelper = new SubscriberListenerHelper();
        }

        internal Subscriber(IntPtr gapiPtr, SubscriberListenerHelper listenerHelper)
            : base(gapiPtr)
        {
            // Base class handles everything.
            this.listenerHelper = listenerHelper;
        }

        public ReturnCode SetListener(ISubscriberListener listener, StatusKind mask)
        {
            ReturnCode result = ReturnCode.Error;


            Gapi.gapi_subscriberListener gapiListener;
            listenerHelper.Listener = listener;
            listenerHelper.CreateListener(out gapiListener);
            if (listener == null)
            {
                using (SubscriberListenerMarshaler marshaler = 
                        new SubscriberListenerMarshaler(ref gapiListener))
                {
                    result = Gapi.Subscriber.set_listener(
                            GapiPeer,
                            marshaler.GapiPtr,
                            mask);
                }
            }
            else
            {
                lock (listener)
                {
                    using (SubscriberListenerMarshaler marshaler = 
                            new SubscriberListenerMarshaler(ref gapiListener))
                    {
                        result = Gapi.Subscriber.set_listener(
                                GapiPeer,
                                marshaler.GapiPtr,
                                mask);
                    }
                }
            }

            return result;
        }

        public IDataReader CreateDataReader(ITopicDescription topic)
        {
            IDataReader dataReader = null;

            if (topic != null)
            {
                SacsSuperClass superObj = (SacsSuperClass)topic;

                IntPtr gapiPtr = Gapi.Subscriber.create_datareader(
                        GapiPeer,
                        superObj.GapiPeer,
                        IntPtr.Zero,
                        IntPtr.Zero,
                        StatusKind.Any);
                if (gapiPtr != IntPtr.Zero)
                {
                    TypeSupport typeSupport = topic.Participant.GetTypeSupport(topic.TypeName) 
                            as OpenSplice.TypeSupport;
                    dataReader = typeSupport.CreateDataReader(gapiPtr);
                }
            }
            return dataReader;
        }

        public IDataReader CreateDataReader(
                ITopicDescription topic,
                IDataReaderListener listener, 
                StatusKind mask)
        {

            DataReader dataReader = null;

            if (topic != null)
            {
                SacsSuperClass superObj = (SacsSuperClass)topic;

                OpenSplice.Gapi.gapi_dataReaderListener gapiListener;
                DataReaderListenerHelper listenerHelper = new DataReaderListenerHelper();
                listenerHelper.Listener = listener;
                listenerHelper.CreateListener(out gapiListener);
                lock (listener)
                {
                    using (DataReaderListenerMarshaler listenerMarshaler = 
                            new DataReaderListenerMarshaler(ref gapiListener))
                    {
                        IntPtr gapiPtr = Gapi.Subscriber.create_datareader(
                                GapiPeer,
                                superObj.GapiPeer,
                                IntPtr.Zero,
                                listenerMarshaler.GapiPtr,
                                mask);
                        if (gapiPtr != IntPtr.Zero)
                        {
                            TypeSupport typeSupport = topic.Participant.GetTypeSupport(topic.TypeName) 
                                    as OpenSplice.TypeSupport;
                            dataReader = typeSupport.CreateDataReader(gapiPtr);
                            dataReader.SetListener(listenerHelper);
                        }
                    }
                }
            }
            return dataReader;
        }

        public IDataReader CreateDataReader(ITopicDescription topic, DataReaderQos qos)
        {
            IDataReader dataReader = null;
            if (topic != null)
            {
                SacsSuperClass superObj = (SacsSuperClass)topic;

                using (DataReaderQosMarshaler marshaler = new DataReaderQosMarshaler())
                {
                    if (marshaler.CopyIn(qos) == ReturnCode.Ok)
                    {
                        IntPtr gapiPtr = Gapi.Subscriber.create_datareader(
                                GapiPeer,
                                superObj.GapiPeer,
                                marshaler.GapiPtr,
                                IntPtr.Zero,
                                StatusKind.Any);
                        if (gapiPtr != IntPtr.Zero)
                        {
                            TypeSupport typeSupport = topic.Participant.GetTypeSupport(topic.TypeName) 
                                    as OpenSplice.TypeSupport;
                            dataReader = typeSupport.CreateDataReader(gapiPtr);
                        }
                    }
                }
            }
            return dataReader;
        }

        public IDataReader CreateDataReader(
                ITopicDescription topic, 
                DataReaderQos qos,
                IDataReaderListener listener, 
                StatusKind mask)
        {
            DataReader dataReader = null;
            
            if (topic != null)
            {
                SacsSuperClass superObj = (SacsSuperClass)topic;

                using (DataReaderQosMarshaler marshaler = new DataReaderQosMarshaler())
                {
                    if (marshaler.CopyIn(qos) == ReturnCode.Ok)
                    {
                        OpenSplice.Gapi.gapi_dataReaderListener gapiListener;
                        DataReaderListenerHelper listenerHelper = new DataReaderListenerHelper();
                        listenerHelper.Listener = listener;
                        listenerHelper.CreateListener(out gapiListener);
                        lock (listener)
                        {
                            using (DataReaderListenerMarshaler listenerMarshaler = 
                                    new DataReaderListenerMarshaler(ref gapiListener))
                            {
                                IntPtr gapiPtr = Gapi.Subscriber.create_datareader(
                                        GapiPeer,
                                        superObj.GapiPeer,
                                        marshaler.GapiPtr,
                                        listenerMarshaler.GapiPtr,
                                        mask);
                                if (gapiPtr != IntPtr.Zero)
                                {
                                    TypeSupport typeSupport = topic.Participant.GetTypeSupport(topic.TypeName) 
                                            as OpenSplice.TypeSupport;
                                    dataReader = typeSupport.CreateDataReader(gapiPtr);
                                    dataReader.SetListener(listenerHelper);
                                }
                            }
                        }
                    }
                }
            }

            return dataReader;
        }

        public ReturnCode DeleteDataReader(IDataReader dataReader)
        {
            ReturnCode result = ReturnCode.BadParameter;
            DataReader dataReaderObj = dataReader as DataReader;

            if (dataReaderObj != null)
            {
                result = Gapi.Subscriber.delete_datareader(
                        GapiPeer,
                        dataReaderObj.GapiPeer);
            }
            return result;
        }

        public IDataReader LookupDataReader(string topicName)
        {
            IntPtr gapiPtr = Gapi.Subscriber.lookup_datareader(
                    GapiPeer,
                    topicName);

            IDataReader dataReader = SacsSuperClass.fromUserData(gapiPtr) as IDataReader;
            return dataReader;
        }

        public ReturnCode DeleteContainedEntities()
        {
            return Gapi.Subscriber.delete_contained_entities(
                GapiPeer,
                null,
                IntPtr.Zero);
        }

        public ReturnCode GetDataReaders(ref IDataReader[] readers)
        {
            return GetDataReaders(ref readers, SampleStateKind.Any, ViewStateKind.Any, InstanceStateKind.Any);
        }

        public ReturnCode GetDataReaders(
                ref IDataReader[] readers, 
                SampleStateKind sampleStates,
                ViewStateKind viewStates, 
                InstanceStateKind instanceStates)
        {
            ReturnCode result = ReturnCode.Error;

            using (SequenceMarshaler<IDataReader, DataReader> marshaler = 
                    new SequenceMarshaler<IDataReader, DataReader>())
            {
                result = Gapi.Subscriber.get_datareaders(
                        GapiPeer,
                        marshaler.GapiPtr,
                        sampleStates,
                        viewStates,
                        instanceStates);

                if (result == ReturnCode.Ok)
                {
                    marshaler.CopyOut(ref readers);
                }
            }

            return result;
        }

        public ReturnCode NotifyDataReaders()
        {
            return Gapi.Subscriber.notify_datareaders(GapiPeer);
        }

        public ReturnCode SetQos(SubscriberQos qos)
        {
            DDS.ReturnCode result;
            
            using (SubscriberQosMarshaler marshaler = new SubscriberQosMarshaler())
            {
                result = marshaler.CopyIn(qos);
                if (result == DDS.ReturnCode.Ok)
                {
                    result = OpenSplice.Gapi.Subscriber.set_qos(
                            GapiPeer,
                            marshaler.GapiPtr);
                }
            }

            return result;
        }

        public ReturnCode GetQos(ref SubscriberQos qos)
        {
            ReturnCode result;

            using (SubscriberQosMarshaler marshaler = new SubscriberQosMarshaler())
            {
                result = OpenSplice.Gapi.Subscriber.get_qos(
                        GapiPeer,
                        marshaler.GapiPtr);

                if (result == ReturnCode.Ok)
                {
                    marshaler.CopyOut(ref qos);
                }
            }

            return result;
        }

        public ReturnCode BeginAccess()
        {
            return OpenSplice.Gapi.Subscriber.begin_access(GapiPeer);
        }

        public ReturnCode EndAccess()
        {
            return OpenSplice.Gapi.Subscriber.end_access(GapiPeer);
        }

        public IDomainParticipant Participant
        {
            get
            {
                IntPtr gapiPtr = Gapi.Subscriber.get_participant(GapiPeer);

                IDomainParticipant domainParticipant = SacsSuperClass.fromUserData(gapiPtr) 
                        as IDomainParticipant;
                return domainParticipant;
            }
        }

        public ReturnCode SetDefaultDataReaderQos(DataReaderQos qos)
        {
            DDS.ReturnCode result;
            
            using (DataReaderQosMarshaler marshaler = new DataReaderQosMarshaler())
            {
                result = marshaler.CopyIn(qos);
                if (result == DDS.ReturnCode.Ok)
                {
                    return OpenSplice.Gapi.Subscriber.set_default_datareader_qos(
                            GapiPeer,
                            marshaler.GapiPtr);
                }
            }

            return result;
        }

        public ReturnCode GetDefaultDataReaderQos(ref DataReaderQos qos)
        {
            ReturnCode result;

            using (DataReaderQosMarshaler marshaler = new DataReaderQosMarshaler())
            {
                result = OpenSplice.Gapi.Subscriber.get_default_datareader_qos(
                    GapiPeer,
                    marshaler.GapiPtr);

                if (result == ReturnCode.Ok)
                {
                    marshaler.CopyOut(ref qos);
                }
            }

            return result;
        }

        public ReturnCode CopyFromTopicQos(ref DataReaderQos dataReaderQos, TopicQos topicQos)
        {
            ReturnCode result = ReturnCode.Ok;

            if (dataReaderQos == null) 
            {
                result = GetDefaultDataReaderQos(ref dataReaderQos);
            }

            if (result == ReturnCode.Ok)
            {
                using (TopicQosMarshaler marshaler = new TopicQosMarshaler())
                {
                    result = marshaler.CopyIn(topicQos);
                    if (result == ReturnCode.Ok)
                    {
                        using (DataReaderQosMarshaler dataReaderMarshaler = 
                                new DataReaderQosMarshaler())
                        {
                            result = dataReaderMarshaler.CopyIn(dataReaderQos);
                            if (result == ReturnCode.Ok)
                            {
                                result = OpenSplice.Gapi.Subscriber.copy_from_topic_qos(
                                        GapiPeer,
                                        dataReaderMarshaler.GapiPtr,
                                        marshaler.GapiPtr);

                                if (result == ReturnCode.Ok)
                                {
                                    dataReaderMarshaler.CopyOut(ref dataReaderQos);
                                }
                            }
                        }
                    }
                }
            }

            return result;
        }
    }
}
