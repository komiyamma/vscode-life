
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

	// SHIFTキーを押しているか、エンジンが立ち上がっていないならば･･･
	if (GetKeyState(VK_SHIFT) < 0 || !hWndSE) {

		// カレントディレクトリ変更
		SetCurrentDirectoryA(szTargetDirName);

		// 起動
		ShellExecuteA(NULL, "open", szTargetExeFullPath, NULL, NULL, SW_SHOWNORMAL);

	}
	else {

		// もしアイコン化していたら元に戻す
		if (IsIconic(hWndSE)) {

			// 元に戻す
			OpenIcon(hWndSE);
		}

		// それがアクティブだ!!
		SetForegroundWindow(hWndSE);
	}

	return 0;
}
