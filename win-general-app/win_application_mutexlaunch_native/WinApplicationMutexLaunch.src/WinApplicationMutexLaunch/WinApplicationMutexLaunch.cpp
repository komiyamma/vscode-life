#include <windows.h>
#include <shlwapi.h>
#include <tchar.h>
#include <vector>
#include <string>


#pragma comment(lib, "shlwapi.lib")

using namespace std;

int APIENTRY WinMain(
	HINSTANCE hInstance,
	HINSTANCE hPrevInstance,
	LPSTR lpCmdLine,
	int nCmdShow)
{
	// [自分自身] [ターゲットプログラムのフルパス] [ターゲットのウィンドウクラス] [そのプログラムへの残りの引数]
	if (__argc < 3) {
		return -1;
	}

	vector<string> args_list;
	string joined_args_list = "";

	// プログラムパス以外の引数あり。
	if (__argc > 3) {
		for (int c = 3; c < __argc; c++) {
			string elem = "\"" + string(__argv[c]) + "\"";
			// ダブルクォーテーションで囲みながら集める
			args_list.push_back(elem);
			joined_args_list += " " + elem;
		}
	}

	char *szTargetExeFullPath = __argv[1];
	char *szTargetWndClassName = __argv[2];
	char szTargetDirName[256] = "";
	strcpy(szTargetDirName, szTargetExeFullPath);
	::PathRemoveFileSpecA(szTargetDirName);

	// FindWindowは、対象のウィンドウクラスが複数ある場合には、Zオーダーが一番上のものを拾う
	HWND hWndHandle = FindWindowA(szTargetWndClassName, NULL);

	// SHIFTキーを押しているか、対象のアプリが立ち上がっていないならば･･･
	if (GetKeyState(VK_SHIFT) < 0 || !hWndHandle) {

		// カレントディレクトリ変更
		SetCurrentDirectoryA(szTargetDirName);

		// 起動
		ShellExecuteA(NULL, "open", szTargetExeFullPath, joined_args_list.data(), NULL, SW_SHOWNORMAL);

	}
	else {

		// もしアイコン化していたら元に戻す
		if (IsIconic(hWndHandle)) {

			// 元に戻す
			OpenIcon(hWndHandle);
		}

		// それがアクティブだ!!
		SetForegroundWindow(hWndHandle);
	}

	return 0;
}