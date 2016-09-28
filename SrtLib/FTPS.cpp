#include<string>
#include"Authorization.cpp"
#include"Exception.cpp"
using namespace System;
using namespace System::Runtime::InteropServices;
using namespace System::Runtime::CompilerServices;
using namespace System::Reflection;

namespace SrtLib {
	public ref class FTPS {
		property Auth^ AuthorizeInfo;

		String^ GetUsr() {
			const std::wstring psw = L"mods.jp-ksprogram";
			return gcnew String(psw.c_str());
		}

		String^ GetPsw() {
			const std::wstring psw = L"Y2unitk1k2";
			return gcnew String(psw.c_str());
		}

		String^ GetFtpAdr() {
			const std::wstring psw = L"ftp://users413.lolipop.jp";
			return gcnew String(psw.c_str());
		}

		String^ GetAddressPHP() {
			const std::wstring psw = L"https://mods-ksprogram.ssl-lolipop.jp/GetAddress.php";
			return gcnew String(psw.c_str());
		}

	public:
		FTPS(Auth^ AuthorizeInfo) {
			this->AuthorizeInfo = AuthorizeInfo;
		}

		void UploadTextToLAPReportServer(String^ Text, String^ Address) {
			if (AuthorizeInfo->IsLAPAuthorized == false) throw gcnew UnauthorizedAccessException();
			System::Net::FtpWebRequest^ ftpReq = (System::Net::FtpWebRequest^)System::Net::WebRequest::Create(GetFtpAdr() + Address);
			ftpReq->Method = System::Net::WebRequestMethods::Ftp::UploadFile;
			ftpReq->KeepAlive = false;
			ftpReq->UseBinary = false;
			ftpReq->UsePassive = true;
			ftpReq->EnableSsl = true;

			ftpReq->Credentials = gcnew System::Net::NetworkCredential(GetUsr(), GetPsw());

			System::IO::Stream^ reqStrm = ftpReq->GetRequestStream();
			System::IO::StringReader^ sr = gcnew System::IO::StringReader(Text);
			cli::array<Byte, 1>^ Buffer = System::Text::Encoding::UTF8->GetBytes(Text);
			reqStrm->Write(Buffer, 0, Buffer->Length);
			reqStrm->Close();

			System::Net::FtpWebResponse^ ftpRes =
				(System::Net::FtpWebResponse^)ftpReq->GetResponse();
			ftpRes->Close();
		}

		String^ GetResponseForLAPReport() {
			if (AuthorizeInfo->IsLAPAuthorized == false) throw gcnew UnauthorizedAccessException();
			System::Net::HttpWebRequest^ req =
				(System::Net::HttpWebRequest^)System::Net::WebRequest::Create(GetAddressPHP() + "?title=LAP_Reports");
			System::Net::HttpWebResponse^ res = (System::Net::HttpWebResponse^)req->GetResponse();

			System::IO::StreamReader^ sr = gcnew System::IO::StreamReader(res->GetResponseStream());
			String^ adr = sr->ReadToEnd();

			sr->Close();
			res->Close();

			return adr;
		}
	};
}