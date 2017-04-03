
#include <windows.h>
#include <shlwapi.h>
#include <tchar.h>

#pragma comment(lib, "shlwapi.lib")

int APIENTRY WinMain(
	HINSTANCE hInstance,
	HINSTANCE hPrevInstance,
	LPSTR lpCmdLine,
	int nCmdShow)
{
	if (__argc < 3) {
		return -1;
	}

	char *szTargetWndClassName = __argv[2];
	char *szTargetExeFullPath = __argv[1];
	char szTargetDirName[256] = "";
	strcpy(szTargetDirName, szTargetExeFullPath);
	::PathRemoveFileSpecA(szTargetDirName);

	HWND hWndSE = FindWindowA(szTargetWndClassName, NULL);

	// SHIFT�L�[�������Ă��邩�A�G���W���������オ���Ă��Ȃ��Ȃ�Υ��
	if (GetKeyState(VK_SHIFT) < 0 || !hWndSE) {

		// �J�����g�f�B���N�g���ύX
		SetCurrentDirectoryA(szTargetDirName);

		// �N��
		ShellExecuteA(NULL, "open", szTargetExeFullPath, NULL, NULL, SW_SHOWNORMAL);

	}
	else {

		// �����A�C�R�������Ă����猳�ɖ߂�
		if (IsIconic(hWndSE)) {

			// ���ɖ߂�
			OpenIcon(hWndSE);
		}

		// ���ꂪ�A�N�e�B�u��!!
		SetForegroundWindow(hWndSE);
	}

	return 0;
}
