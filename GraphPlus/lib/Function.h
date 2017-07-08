//using namespace System;
#include <string>
#using <mscorlib.dll>
#include <msclr/marshal_cppstd.h>
#include <list>
using namespace std;
#pragma once


	class FunctionC
	{
	public:
		FunctionC(string);
		float Calculate(float);
		string functionString;
	private:
		vector<string> Split(string input, char separator);
		int GetLevel(char op);
		float Operate(float a, float b, char op);
		bool IsOperator(char op);

	};




/*
ref class Function
{
public:
	Function(System::String^);
	float Calculate(float);
	//String^ result;
	//FunctionC *F;
};


*/