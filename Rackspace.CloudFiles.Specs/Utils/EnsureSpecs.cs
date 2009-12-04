using System;
using NUnit.Framework;
using Rackspace.CloudFiles.exceptions;
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
           Asserts.Throws<ArgumentNullException>(()=>new Ensure().NotNullOrEmpty(foo, foobar));
        }
    }
    [TestFixture]
    public class When_a_container_name_has_no_invalid_characters
    {
        [Test]
        public void should_be_valid()
        {
            var containerName = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789abcdefghijklmnopqrstuvwxyz!@#$%^&*()-+={}[]|\'\":;,.<>~`";
            new Ensure().ValidContainerName(containerName);

        }
    }

    [TestFixture]
    public class When_a_container_name_has_a_slash
    {
        [Test]
        public void should_be_invalid()
        {
            var containerName = "containerName/withSlash";
           Asserts.Throws<InvalidContainerNameException>( ()=>new Ensure().ValidContainerName(containerName));

        }
    }

    [TestFixture]
    public class When_a_container_name_has_a_question_mark
    {
        [Test]
        public void should_be_invalid()
        {
            var containerName = "containerName?withQuestionMark";
            Asserts.Throws<InvalidContainerNameException>(() => new Ensure().ValidContainerName(containerName));

        }
    }

    [TestFixture]
    public class When_a_container_name_is_valid_length
    {
        [Test]
        public void should_be_valid()
        {
            var containerName = CreateString.WithLength(256);
            new Ensure().ValidContainerName(containerName);

        }
    }

    [TestFixture]
    public class When_a_container_name_is_invalid_length
    {
        [Test]
        public void should_be_invalid()
        {
            var containerName = CreateString.WithLength(257);
            Asserts.Throws<InvalidContainerNameException>(()=>new Ensure().ValidContainerName(containerName));

        }
    }
    [TestFixture]
    public class When_a_object_name_has_no_invalid_characters
    {
        [Test]
        public void should_be_valid()
        {
            var objectName = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789abcdefghijklmnopqrstuvwxyz!@#$%^&*()-+={}[]|\'\":;,.<>~`/";
            new Ensure().ValidStorageObjectName(objectName);

        }
    }

    [TestFixture]
    public class When_a_object_name_has_a_question_mark
    {
        [Test]
        public void should_be_invalid()
        {
            var objectName = "objectName?withQuestionMark";
            Asserts.Throws<InvalidStorageObjectNameException>(() => new Ensure().ValidStorageObjectName(objectName));


        }
    }

    [TestFixture]
    public class When_a_object_name_is_valid_length
    {
        [Test]
        public void should_be_valid()
        {
            var objectName = CreateString.WithLength(1024);
            new Ensure().ValidStorageObjectName(objectName);

        }
    }

    [TestFixture]
    public class When_a_object_name_is_invalid_length
    {
        [Test]
        public void should_be_invalid()
        {
            var objectName = CreateString.WithLength(1025);
            Asserts.Throws<InvalidStorageObjectNameException>(() => new Ensure().ValidStorageObjectName(objectName));

        }
    }
}