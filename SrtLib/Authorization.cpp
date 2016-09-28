#include<string>
using namespace System;
using namespace System::Reflection;
using namespace System::Diagnostics;

namespace SrtLib {
	const std::wstring Original = L"28C42072F5B291F51D4937CD86F93AB54E348CDA24F3BA77D9603DF25FD12DAA";

	public ref class Auth {
	internal:
		Boolean IsLAPAuthorized = false;
	public:
		void AuthorizeLAP() {
			try {
				cli::array<Byte, 1>^ hash = ComputeHash(gcnew System::Security::Cryptography::SHA256Managed(),
					Process::GetCurrentProcess()->Modules[0]->FileName);
				String^ hashstr = BitConverter::ToString(hash)->Replace("-", "");

				Console::WriteLine(hashstr);

				String^ act = gcnew String(Original.c_str());
				if (act == hashstr) {
					IsLAPAuthorized = true;
				}

				Console::WriteLine(IsLAPAuthorized);
			}
			catch (Exception^ ex) {
				Console::WriteLine(ex->Message);
			}

#ifdef _DEBUG
			IsLAPAuthorized = true;
#endif
		}

		Boolean GetAuthorizeState() {
			return IsLAPAuthorized;
		}

		cli::array<unsigned char, 1>^ ComputeHash(System::Security::Cryptography::HashAlgorithm^ Algorithm, String^ FilePath) {
			System::IO::FileStream^ fs = gcnew System::IO::FileStream(FilePath,
				System::IO::FileMode::Open, System::IO::FileAccess::Read, System::IO::FileShare::ReadWrite);

			String^ hashstr = "";
			cli::array<unsigned char, 1>^ hash = Algorithm->ComputeHash(fs);

			delete Algorithm;
			fs->Close();

			return hash;
		}
	};
}