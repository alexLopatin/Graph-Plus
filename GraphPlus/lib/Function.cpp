#include <string>
#using <mscorlib.dll>
#include <msclr/marshal_cppstd.h>
#include <list>


using namespace System::Collections::Generic;
using namespace System::Collections;
using namespace System;

class FunctionC
{
public:
	std::string functionString;
	FunctionC(std::string FunctionString)
	{
		functionString = FunctionString;
	}
	float Calculate(float x)
	{
		std::vector<float> nums;
		auto ops = Split(functionString, ' ');

		for (int i = 0; i < ops.size(); i++)
			if (ops[i] == "x")
				ops[i] = std::to_string(x);
		int f = ops.size();

		for (int i = 0; i < ops.size(); i++)
		{
			if (!IsOperator(ops[i][0]))
				nums.insert(nums.begin(), std::strtof(ops[i].c_str(), NULL));
			else
			{
				float a = nums[0];
				float b = nums[1];
				float res = Operate(a, b, ops[i][0]);
				nums.erase(nums.begin() + 1);
				nums.erase(nums.begin());
				nums.insert(nums.begin(), res);
			}
		}
		return nums[0];
	}
private:
	std::vector<std::string> Split(std::string input, char separator)
	{
		std::vector<std::string> result;
		std::string val = "";
		for (int i = 0; i < input.length(); i++)
		{
			if (input[i] != separator)
			{
				char c = input[i];
				val += input[i];
			}

			else
			{
				if (val != "")
					result.push_back(val);
				val = "";
			}
		}
		if (val != "")
			result.push_back(val);
		int f = result.size();


		return result;
	}
	int GetLevel(char op)
	{
		switch (op)
		{
		case '(':
			return 0;
			break;
		case ')':
			return 1;
			break;
		case '+':
			return 2;
			break;
		case '-':
			return 2;
			break;
		case '*':
			return 3;
			break;
		case '/':
			return 3;
			break;
		case '^':
			return 4;
			break;

		default:
			return -1;
			break;
		}
	}
	float Operate(float a, float b, char op)
	{
		switch (op)
		{
		case '+':
			return a + b;
			break;
		case '-':
			return b - a;
			break;
		case '*':
			return a*b;
			break;
		case '/':
			return b / a;
			break;
		case '^':
			return Math::Pow(b, a);
			break;
		default:

			break;
		}
		return 0;
	}
	bool IsOperator(char op)
	{
		if (GetLevel(op) == -1)
			return false;
		else
			return true;
	}
};
