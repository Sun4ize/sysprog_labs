#include <iostream>
#include <thread>
#include <vector>
#include <Windows.h>
#include <conio.h>
#include <sstream>

std::string strtr(int i)
{
    std::string s;
    std::stringstream out;
    out << i;
    s = out.str();
    return s;
}


DWORD WINAPI LyaskinThread(LPVOID lpParameter)
{
    int id = int(lpParameter);
    auto str = "EventStop" + strtr(id);
    HANDLE hMutex = CreateMutex(NULL, FALSE, "LyaskinMutex");
    HANDLE hEventPrint = CreateEvent(NULL, TRUE, FALSE, "EventPrint");
    HANDLE hEventStop = CreateEvent(NULL, TRUE, FALSE, str.c_str());
    WaitForSingleObject(hMutex, INFINITE);
    std::cout << "\nThread started " << id << std::endl;
    ReleaseMutex(hMutex);
    HANDLE hEvents[] = { hEventStop };
    while (true) 
    {
        switch (WaitForMultipleObjects(1, hEvents, FALSE, INFINITE) - WAIT_OBJECT_0)
        {
            case 0:
            {
                ResetEvent(hEventStop);
                WaitForSingleObject(hMutex, INFINITE);
                std::cout << "\nthread ended  " << id << std::endl;
                ReleaseMutex(hMutex);
                return 0;
            }
              
        }
    }
    return 0;
}


void start() 
{
    SetPriorityClass(GetCurrentProcess(), REALTIME_PRIORITY_CLASS);
    auto hMutex = CreateMutex(NULL, FALSE, "LyaskinMutex");
    auto hEventStart = CreateEvent(NULL, TRUE, FALSE, "EventStart");
    auto hEventStop = CreateEvent(NULL, TRUE, FALSE, "EventStop");
    auto evSubmit = CreateEvent(NULL, TRUE, FALSE, "ThreadSubmit");
    HANDLE hEvents[] = { hEventStart, hEventStop, };
    int i = 0;

    while (true)
    {
        switch (WaitForMultipleObjects(2, hEvents, FALSE, INFINITE) - WAIT_OBJECT_0)
        {
        case 0:
        {
            ResetEvent(hEventStart);
            HANDLE hThread = CreateThread(NULL, 0, LyaskinThread, (LPVOID)i++, 0, NULL);
            SetEvent(evSubmit);
            break;
        }
        case 1:
        {
            if (i == 0)
            {
                return;
            }
            else
            {
                auto str = "EventStop" + strtr(i--);
                HANDLE hlEvenStop = CreateEvent(NULL, TRUE, FALSE, str.c_str());
                SetEvent(hlEvenStop);
                ResetEvent(hlEvenStop);
                SetEvent(evSubmit);
            }
        }       
        }
    }
    

    while (true) 
    {
        SetEvent(hEventStart); 
        ResetEvent(hEventStart);
    }

    CloseHandle(hMutex);
    CloseHandle(hEventStart);
    CloseHandle(hEventStop);
    CloseHandle(evSubmit);
}



int main()
{
    start();
    return 0;
}
