// EndPointController.cpp : Defines the entry point for the console application.
//
#include <stdio.h>
#include <wchar.h>
#include <tchar.h>
#include "windows.h"
#include "Mmdeviceapi.h"
#include "PolicyConfig.h"
#include "Propidl.h"
#include "Functiondiscoverykeys_devpkey.h"

HRESULT SetDefaultAudioPlaybackDevice(LPCWSTR devID)
{	
	IPolicyConfigVista *pPolicyConfig;
	ERole reserved = eConsole;

    HRESULT hr = CoCreateInstance(__uuidof(CPolicyConfigVistaClient), 
		NULL, CLSCTX_ALL, __uuidof(IPolicyConfigVista), (LPVOID *)&pPolicyConfig);
	if (SUCCEEDED(hr))
	{
		hr = pPolicyConfig->SetDefaultEndpoint(devID, reserved);
		pPolicyConfig->Release();
	}
	return hr;
}

int ChangeAudioDevice(int option) {
	HRESULT hr = CoInitialize(NULL);
	printf("S_FALSE: %d\n", S_FALSE);
	printf("RPC_E_CHANGED_MODE: %d\n", RPC_E_CHANGED_MODE);
	printf("HR: %d\n", hr);
	IMMDeviceEnumerator *pEnum = NULL;
	// Create a multimedia device enumerator.
	hr = CoCreateInstance(__uuidof(MMDeviceEnumerator), NULL,
		CLSCTX_ALL, __uuidof(IMMDeviceEnumerator), (void**)&pEnum);
	if (SUCCEEDED(hr))
	{
		IMMDeviceCollection *pDevices;
		// Enumerate the output devices.
		hr = pEnum->EnumAudioEndpoints(eRender, DEVICE_STATE_ACTIVE, &pDevices);
		if (SUCCEEDED(hr))
		{
			UINT count;
			pDevices->GetCount(&count);
			if (SUCCEEDED(hr))
			{
				for (unsigned int i = 0; i < count; i++)
				{
					IMMDevice *pDevice;
					hr = pDevices->Item(i, &pDevice);
					if (SUCCEEDED(hr))
					{
						LPWSTR wstrID = NULL;
						hr = pDevice->GetId(&wstrID);
						if (SUCCEEDED(hr))
						{
							IPropertyStore *pStore;
							hr = pDevice->OpenPropertyStore(STGM_READ, &pStore);
							if (SUCCEEDED(hr))
							{
								PROPVARIANT friendlyName;
								PropVariantInit(&friendlyName);
								hr = pStore->GetValue(PKEY_Device_FriendlyName, &friendlyName);
								if (SUCCEEDED(hr))
								{
									// if no options, print the device
									// otherwise, find the selected device and set it to be default
									if (option == -1) printf("Audio Device %d: %ws\n", i, friendlyName.pwszVal);
									if (i == option) SetDefaultAudioPlaybackDevice(wstrID);
									PropVariantClear(&friendlyName);
								}
								pStore->Release();
							}
						}
						pDevice->Release();
					}
				}
			}
			pDevices->Release();
		}
		pEnum->Release();
	}

	return hr;
}

void ChangeAudioDeviceByName(TCHAR* devName) {
	HRESULT hr = CoInitialize(NULL);

	IMMDeviceEnumerator *pEnum = NULL;
	// Create a multimedia device enumerator.
	hr = CoCreateInstance(__uuidof(MMDeviceEnumerator), NULL,
		CLSCTX_ALL, __uuidof(IMMDeviceEnumerator), (void**)&pEnum);
	if (SUCCEEDED(hr))
	{
		IMMDeviceCollection *pDevices;
		// Enumerate the output devices.
		hr = pEnum->EnumAudioEndpoints(eRender, DEVICE_STATE_ACTIVE, &pDevices);
		if (SUCCEEDED(hr))
		{
			UINT count;
			pDevices->GetCount(&count);
			if (SUCCEEDED(hr))
			{
				for (unsigned int i = 0; i < count; i++)
				{
					IMMDevice *pDevice;
					hr = pDevices->Item(i, &pDevice);
					if (SUCCEEDED(hr))
					{
						LPWSTR wstrID = NULL;
						hr = pDevice->GetId(&wstrID);

						if (SUCCEEDED(hr))
						{
							IPropertyStore *pStore;
							hr = pDevice->OpenPropertyStore(STGM_READ, &pStore);
							if (SUCCEEDED(hr))
							{
								PROPVARIANT friendlyName;
								PropVariantInit(&friendlyName);
								hr = pStore->GetValue(PKEY_Device_FriendlyName, &friendlyName);
								if (SUCCEEDED(hr))
								{
									// if no options, print the device
									// otherwise, find the selected device and set it to be default
									//wprintf(L"comparing %s vs %s\n", friendlyName.pwszVal, devName);
									if (NULL != wcsstr(friendlyName.pwszVal, devName))
									{
										SetDefaultAudioPlaybackDevice(wstrID);
										wprintf(L"[LIB] changed audio device to %s\n", friendlyName.pwszVal);
									}
									PropVariantClear(&friendlyName);
								}
								else { printf("fail 1\n"); }
								pStore->Release();
							}
							else { printf("fail 2\n"); }
						}
						else { printf("fail 3\n"); }
						pDevice->Release();
					}
					else { printf("fail 4\n"); }
				}
			}
			else { printf("fail 5\n"); }
			pDevices->Release();
		}
		else { printf("fail 6\n"); }
		pEnum->Release();
	}
	else { printf("fail 7\n"); }
}


void ChangeAudioDeviceByID(TCHAR* devID) {
	HRESULT hr = CoInitialize(NULL);

	IMMDeviceEnumerator *pEnum = NULL;
	// Create a multimedia device enumerator.
	hr = CoCreateInstance(__uuidof(MMDeviceEnumerator), NULL,
		CLSCTX_ALL, __uuidof(IMMDeviceEnumerator), (void**)&pEnum);
	if (SUCCEEDED(hr))
	{
		IMMDeviceCollection *pDevices;
		// Enumerate the output devices.
		hr = pEnum->EnumAudioEndpoints(eRender, DEVICE_STATE_ACTIVE, &pDevices);
		if (SUCCEEDED(hr))
		{
			UINT count;
			pDevices->GetCount(&count);
			if (SUCCEEDED(hr))
			{
				for (unsigned int i = 0; i < count; i++)
				{
					IMMDevice *pDevice;
					hr = pDevices->Item(i, &pDevice);
					if (SUCCEEDED(hr))
					{
						LPWSTR wstrID = NULL;
						hr = pDevice->GetId(&wstrID);

						if (SUCCEEDED(hr))
						{
							IPropertyStore *pStore;
							hr = pDevice->OpenPropertyStore(STGM_READ, &pStore);
							if (SUCCEEDED(hr))
							{
								PROPVARIANT friendlyName;
								PropVariantInit(&friendlyName);
								hr = pStore->GetValue(PKEY_Device_FriendlyName, &friendlyName);
								if (SUCCEEDED(hr))
								{
									// if no options, print the device
									// otherwise, find the selected device and set it to be default
									//wprintf(L"comparing %s vs %s\n", friendlyName.pwszVal, devName);
									//wprintf(L"%s -> %s\n", friendlyName.pwszVal, wstrID);
									if (0 == wcscmp(wstrID, devID))
									{
										SetDefaultAudioPlaybackDevice(wstrID);
										wprintf(L"[LIB] changed audio device to %s\n", friendlyName.pwszVal);
									}
									PropVariantClear(&friendlyName);
								}
								else { printf("fail 1\n"); }
								pStore->Release();
							}
							else { printf("fail 2\n"); }
						}
						else { printf("fail 3\n"); }
						pDevice->Release();
					}
					else { printf("fail 4\n"); }
				}
			}
			else { printf("fail 5\n"); }
			pDevices->Release();
		}
		else { printf("fail 6\n"); }
		pEnum->Release();
	}
	else { printf("fail 7\n"); }
}



// EndPointController.exe [NewDefaultDeviceID]
int main(int argc, char* argv[])
{
	// read the command line option, -1 indicates list devices.
	int option = -1;
	if (argc == 2) option = atoi(argv[1]);

	ChangeAudioDevice(option);

	return 0;
}
