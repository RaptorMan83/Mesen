#pragma once

#include "stdafx.h"
#include "CPU.h"
#include "PPU.h"
#include "APU.h"
#include "MemoryManager.h"
#include "ControlManager.h"
#include "../Utilities/SimpleLock.h"
#include "AutoSaveManager.h"

class Debugger;
class BaseMapper;

class Console
{
	private:
		static shared_ptr<Console> Instance;
		SimpleLock _pauseLock;
		SimpleLock _runLock;
		SimpleLock _stopLock;

		shared_ptr<CPU> _cpu;
		shared_ptr<PPU> _ppu;
		unique_ptr<APU> _apu;
		shared_ptr<Debugger> _debugger;
		SimpleLock _debuggerLock;
		shared_ptr<BaseMapper> _mapper;
		unique_ptr<ControlManager> _controlManager;
		shared_ptr<MemoryManager> _memoryManager;

		unique_ptr<AutoSaveManager> _autoSaveManager;

		NesModel _model;

		string _romFilepath;

		bool _stop = false;
		bool _reset = false;

		atomic<bool> _resetRequested = false;
		atomic<uint32_t> _lagCounter;
		
		bool _initialized = false;

		void ResetComponents(bool softReset);
		void Initialize(string filename, stringstream *filestream = nullptr, string ipsFilename = "", int32_t archiveFileIndex = -1);
		void UpdateNesModel(bool sendNotification);
		double GetFrameDelay();

	public:
		Console();
		~Console();
		void Run();
		void Stop();
		static void RequestReset();
		static void Reset(bool softReset = true);

		//Used to pause the emu loop to perform thread-safe operations
		static void Pause();

		//Used to resume the emu loop after calling Pause()
		static void Resume();

		std::shared_ptr<Debugger> GetDebugger(bool autoStart = true);
		void StopDebugger();

		static NesModel GetNesModel();
		static void SaveState(ostream &saveStream);
		static void LoadState(istream &loadStream);
		static void LoadState(uint8_t *buffer, uint32_t bufferSize);

		static void LoadROM(string filepath, stringstream *filestream = nullptr, int32_t archiveFileIndex = -1, string ipsFile = "");
		static bool LoadROM(string romName, uint32_t crc32Hash);
		static string GetROMPath();
		static string GetRomName();
		static uint32_t GetCrc32();
		static uint32_t GetPrgCrc32();
		static NesModel GetModel();

		static uint32_t GetLagCounter();
		static void ResetLagCounter();

		static bool IsRunning();

		static shared_ptr<Console> GetInstance();
		static void Release();
};
