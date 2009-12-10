using System;
using NUnit.Framework;
using Rackspace.CloudFiles.exceptions;
using Rackspace.CloudFiles.Specs.CustomAsserts;
using Rackspace.CloudFiles.utils;

namespace Rackspace.CloudFiles.Specs.Utils
{

    [TestFixture]
    public class EnsureNotNullOrEmptyWhenStringsAreNull
    {
        [Test]
        public void it_throws_EnsureException()
        {
            string foo = null;
            string foobar = null;
           Asserts.Throws<ArgumentNullException>(()=>Ensure.NotNullOrEmpty(foo, foobar));
        }
    }
    [TestFixture]
    public class When_a_container_name_has_no_invalid_characters
    {
        [Test]
        public void should_be_valid()
        {
            var containerName = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789abcdefghijklmnopqrstuvwxyz!@#$%^&*()-+={}[]|\'\":;,.<>~`";
            Ensure.ValidContainerName(containerName);

        }
    }

    [TestFixture]
    public class When_a_container_name_has_a_slash
    {
        [Test]
        public void should_be_invalid()
        {
            var containerName = "containerName/withSlash";
           Asserts.Throws<InvalidContainerNameException>( ()=>Ensure.ValidContainerName(containerName));

        }
    }

    [TestFixture]
    public class When_a_container_name_has_a_question_mark
    {
        [Test]
        public void should_be_invalid()
        {
            var containerName = "containerName?withQuestionMark";
            Asserts.Throws<InvalidContainerNameException>(() =>  Ensure.ValidContainerName(containerName));

        }
    }

    [TestFixture]
    public class When_a_container_name_is_valid_length
    {
        [Test]
        public void should_be_valid()
        {
            var containerName = CreateString.WithLength(128);
            Ensure.ValidContainerName(containerName);

        }
    }

    [TestFixture]
    public class When_a_container_name_is_invalid_length
    {
        [Test]
        public void should_be_invalid()
        {
            var containerName = CreateString.WithLength(129);
            Asserts.Throws<InvalidContainerNameException>(()=> Ensure.ValidContainerName(containerName));

        }
    }
    [TestFixture]
    public class When_a_object_name_has_no_invalid_characters
    {
        [Test]
        public void should_be_valid()
        {
            var objectName = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789abcdefghijklmnopqrstuvwxyz!@#$%^&*()-+={}[]|\'\":;,.<>~`/";
             Ensure.ValidStorageObjectName(objectName);

        }
    }

    [TestFixture]
    public class When_a_object_name_has_a_question_mark
    {
        [Test]
        public void should_be_invalid()
        {
            var objectName = "objectName?withQuestionMark";
            Asserts.Throws<InvalidStorageObjectNameException>(() =>  Ensure.ValidStorageObjectName(objectName));


        }
    }

    [TestFixture]
    public class When_a_object_name_is_valid_length
    {
        [Test]
        public void should_be_valid()
        {
            var objectName = CreateString.WithLength(1024);
             Ensure.ValidStorageObjectName(objectName);

        }
    }

    [TestFixture]
    public class When_a_object_name_is_invalid_length
    {
        [Test]
        public void should_be_invalid()
        {
            var objectName = CreateString.WithLength(1025);
            Asserts.Throws<InvalidStorageObjectNameException>(() => Ensure.ValidStorageObjectName(objectName));

        }
    }
}