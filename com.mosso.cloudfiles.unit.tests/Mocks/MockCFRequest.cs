using System;
using System.IO;
using System.Net;
using Rackspace.CloudFiles.domain.request.Interfaces;
using Rackspace.CloudFiles.domain.response.Interfaces;

namespace Rackspace.CloudFiles.unit.tests
{
class MockCFRequest: ICloudFilesRequest
	{
		#region ICloudFilesRequest implementation
		public ICloudFilesResponse GetResponse ()
		{
			throw new System.NotImplementedException();
		}
		
		public System.IO.Stream GetRequestStream ()
		{
			return _fakestream;
		}
		private Stream _fakestream;
		
		public void SetContent (System.IO.Stream stream, Connection.ProgressCallback progress)
		{
			_fakestream = stream;
		}
		
		public Uri RequestUri {
			get {
				throw new System.NotImplementedException();
			}
		}
		
		public string Method {
			set;get;
		}
		private WebHeaderCollection _headers = new WebHeaderCollection();
		public System.Net.WebHeaderCollection Headers {
			get {
			 return _headers;
			}
		}
		
		public long ContentLength {
			get {
				throw new System.NotImplementedException();
			}
		}
		
		public int RangeTo {
			get {
				throw new System.NotImplementedException();
			}
			set {
				throw new System.NotImplementedException();
			}
		}
		
		public int RangeFrom {
			get {
				throw new System.NotImplementedException();
			}
			set {
				throw new System.NotImplementedException();
			}
		}
		
		public string ContentType {
			get {
				throw new System.NotImplementedException();
			}
			set {
				throw new System.NotImplementedException();
			}
		}
		
		public DateTime IfModifiedSince {
			get {
				throw new System.NotImplementedException();
			}
			set {
				throw new System.NotImplementedException();
			}
		}
		
		public string ETag {
			get {
				throw new System.NotImplementedException();
			}
		}
		
		public bool AllowWriteStreamBuffering {
			get {
				throw new System.NotImplementedException();
			}
			set {
				throw new System.NotImplementedException();
			}
		}
		
		public bool SendChunked {
			get {
				throw new System.NotImplementedException();
			}
			set {
				throw new System.NotImplementedException();
			}
		}
		
		public System.IO.Stream ContentStream {
			get {
				throw new System.NotImplementedException();
			}
		}
		#endregion
		
		
	}
}
