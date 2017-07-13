#include <d2d1.h>
#include <dwrite.h>
#include <dwrite_1.h>
#include <dwrite_2.h>
#include <array>
#include <wchar.h>
#include <strsafe.h>
#include <string>
#include <math.h>
#include <WinUser.h>
#include <d2d1_1.h>
#include <d2d1_1helper.h>
#include <d2d1effects.h>
#include <d2d1effects_1.h>
//#include <Function.h>
#include <vector>
#include <msclr/marshal_cppstd.h>
#pragma comment(lib, "Dwrite")

using namespace System;
using namespace System::Collections::Generic;

namespace lib
{
	class Operation
	{
	public:
		Operation()
		{
			IsX = true;
			IsNum = false;
			IsOp = false;
		}
		Operation(float n)
		{
			IsX = false;
			IsNum = true;
			IsOp = false;
			num = n;
		}
		Operation(Operation a, Operation b, std::string op)
		{
			IsX = false;
			IsNum = false;
			IsOp = true;
			OpA = new Operation(a);
			OpB = new Operation(b);
			opS = op;
		}
		float Calculate(float x)
		{
			if (IsX)
				return x;
			if (IsNum)
				return num;
			if (IsOp)
				return Operate(OpA->Calculate(x), OpB->Calculate(x));

			return 0;
		}
	private:
		bool IsX;
		bool IsNum;
		bool IsOp;
		float num = 0;
		Operation* OpA;
		Operation* OpB;
		std::string opS;
		float Operate(float a, float b)
		{
			if (opS == "+")
				return a + b;
			else
			if (opS == "-")
				return b - a;
			else
			if (opS == "*")
				return b * a;
			else
			if (opS == "/")
				return b / a;
			else
			if (opS == "^")
				return Math::Pow(b, a);

			return 0;
		}

	};
	class FunctionC
	{
	public:
		std::string functionString;
		FunctionC(std::string FunctionString)
		{
			functionString = FunctionString;
			ops = Split(functionString, ' ');
			GetCountOfNums();
			InitNums();
			nums.reserve(100);
			Initialize();
		}
		
		std::vector<float> nums;

		float Calculate(float x)
		{
			
			return FinalOp.Calculate(x);
		}
		float thickness = 1;
		/*
		
		float Calculate(float x)
		{
		nums.clear();
			for (int i = 0; i < ops.size(); i++)
				if (!IsOperator(ops[i]))
					if (ops[i] == "x")
						nums.push_back(x);
					else
						nums.push_back( nums_[i]);
				else
				{
					float a = nums.back();
					nums.pop_back();
					float b = nums.back();
					nums.pop_back();
					float res = Operate(a, b, ops[i]);
					nums.push_back(res);
				}

		return nums.back();
		}
		*/
		bool IsOperator(std::string op)
		{
			if (GetLevel(op) == -1)
				return false;
			else
				return true;
		}
		
		void Initialize()
		{
			std::vector<Operation> operations;
			
			for (int i = 0; i < ops.size(); i++)
				if (!IsOperator(ops[i]))
					if (ops[i] == "x")
					{
						Operation* o = new Operation();

						operations.push_back(*o);
					}
						
					else
					{
						Operation* o = new Operation(nums_[i]);
						operations.push_back(*o);
					}
						
				else
				{
					Operation a = operations.back();
					operations.pop_back();
					Operation b = operations.back();
					operations.pop_back();

					Operation* o = new Operation(a, b, ops[i]);

					operations.push_back(*o);
				}

			FinalOp = operations.back();
		}
		
	private:
		Operation FinalOp;
		void GetCountOfNums()
		{
			numsCount = 0;
			for (int i = 0; i < ops.size(); i++)
			{
				if (!IsOperator(ops[i]))
					numsCount++;
			}
		}

		std::vector<float> nums_;
		void InitNums()
		{
			std::vector<float> t(ops.size());
			for (int i = 0; i < ops.size(); i++)
				if (!IsOperator(ops[i]))
					t[i] = std::strtof(ops[i].c_str(), NULL);
			nums_ = t;
		}
		int numsCount;
		std::vector<std::string> ops;
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
		int GetLevel(std::string op)
		{
			if(op == "(")
				return 0;
			else
			if (op == ")")
				return 1;
			else
			if (op == "+")
				return 2;
			else
			if (op == "-")
				return 2;
			else
			if (op == "*")
				return 3;
			else
			if (op == "/")
				return 3;
			else
			if (op == "^")
				return 4;

			return -1;
		}
		float Operate(float a, float b, std::string op)
		{
			if(op=="+")
				return a + b;
			else
			if (op == "-")
				return b-a;
			else
			if (op == "*")
				return b * a;
			else
			if (op == "/")
				return b / a;
			else
			if (op == "^")
				return Math::Pow(b, a);

			return 0;
		}
		
	};
	public ref class Function
	{
	public:
		property String^ StringFunc;
		FunctionC *F;
		Function(String^ functionString)
		{
			result = "";
			Stack = gcnew List<System::Char>();
			Parse(functionString);
			StringFunc = functionString;
			F = new FunctionC(msclr::interop::marshal_as< std::string >(result));
		}

		float Calculate(float x)
		{
			return F->Calculate(x);
		}
		void SetThickness(float thickness)
		{
			F->thickness = thickness;
		}

		~Function()
		{
			delete F;
		}
		property String^ result;
	private:
		int GetLevel(Char op)
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

		float Operate(float a, float b, Char op)
		{
			switch (op)
			{
			case '+':
				return a + b;
				break;
			case '-':
				return a - b;
				break;
			case '*':
				return a*b;
				break;
			case '/':
				return a / b;
				break;
			case '^':
				return Math::Pow(a, b);
				break;
			default:

				break;
			}
			return 0;
		}

		bool isDigitOrDot(Char op)
		{
			if ((op >= 48 & op <= 57) | (op == '46'))
				return true;
			else
				return false;
		}
		bool isOperator(Char op)
		{
			if (GetLevel(op) != -1)
				return true;
			else
				return false;
		}
		property List<System::Char>^ Stack;



		bool IsASpecChar(Char s)
		{

			return s == '@';
		};

		void Clear()
		{

			//Stack->RemoveAll(gcnew Predicate<Char>(IsASpecChar));
			for (int i = 0; i < Stack->Count; i++)
			{
				if (i >= Stack->Count)
					break;
				else if (Stack[i] == '@')
				{
					Stack->RemoveAt(i);
					i = -1;
				}


			}
		}

		void Parse(String^ functionString)
		{
			for (int i = 0; i < functionString->Length; i++)
			{
				if (isOperator(functionString[i]))
				{
					if (Stack->Count == 0)
						Stack->Insert(0, functionString[i]);
					else if (functionString[i] == '(')
						Stack->Insert(0, functionString[i]);
					else if (functionString[i] == ')')
					{
						for (int j = 0; j < Stack->Count; j++)
							if (Stack[j] != '(')
							{
								result += " " + Stack[j];
								Stack[j] = '@';
							}
							else
							{
								Stack[j] = '@';
								break;
							}

					}
					else
					{
						for (int j = 0; j < Stack->Count; j++)
							if (Stack[j] == '(')
								break;
							else
								if (GetLevel(Stack[j]) >= GetLevel(functionString[i]))
								{
									result += " " + Stack[j];
									Stack[j] = '@';
								}
						Stack->Insert(0, functionString[i]);
					}
					Clear();
				}
				else
				{
					String^ var = " ";
					for (int j = i; j < functionString->Length; j++)
					{
						if (isOperator(functionString[j]))
						{
							if (j != functionString->Length - 1)
								i = j - 1;
							break;
						}
						else
						{
							if (j == functionString->Length - 1)
								i = j;
							var += functionString[j];
						}

					}
					result += var;

				}

			}
			if (Stack->Count != 0)
			{
				for (int i = 0; i < Stack->Count; i++)
				{
					result += " " + Stack[i];
					//Stack->RemoveAt(i);
				}

			}
		}
	};

	class Renderer
	{
	public:
		HWND Handle;
		
		~Renderer()
		{
			if (factory) factory->Release();
			if (target) target->Release();
			if (m_pBlackBrush) m_pBlackBrush->Release();
			if (m_pLightGrayBrush) m_pLightGrayBrush->Release();
			if (m_pTextFormat) m_pTextFormat->Release();
			if (m_pDWriteFactory) m_pDWriteFactory->Release();
			if (m_pTextLayout) m_pTextLayout->Release();
		}
		
		ID2D1SolidColorBrush *m_pBlackBrush;
		ID2D1SolidColorBrush *m_pLightGrayBrush;
		IDWriteTextFormat *m_pTextFormat;
		IDWriteTextFormat *m_pTextFormat15;
		IDWriteFactory  *m_pDWriteFactory;
		IDWriteTextLayout *m_pTextLayout;
		int k = 0;
		int currentK = 0;


		std::vector<FunctionC*> functions;

		void DrawFunctions()
		{
			if(functions.size()>0)
			for (int i = 0; i < functions.size(); i++)
			{
				RECT rect;
				GetClientRect(Handle, &rect);
				POINT oldDot;
				ID2D1SolidColorBrush* m_pColorBrush;
				target->CreateSolidColorBrush(D2D1::ColorF(D2D1::ColorF::Black, 1.0f),
					&m_pColorBrush
				);
				float transX = CurrentTrans._31;
				float transY = CurrentTrans._32;
				for (int x = -transX; x < -transX + rect.right; x+=3)
				{
					
					POINT newDot;
					newDot.x = x;
					newDot.y = 0;
					newDot.y = -( functions[i]->Calculate(x*pow(10, currentK) / (float)currentValOfDivision) )*currentValOfDivision/pow(10, currentK);

					if (x != -transX)
						target->DrawLine(D2D1::Point2F(oldDot.x, oldDot.y), D2D1::Point2F(newDot.x, newDot.y), m_pColorBrush, functions[i]->thickness);


					oldDot = newDot;
					
					//functions[i]->Calculate(x);
				}
				m_pColorBrush->Release();
			}
		}

		bool Initialize(HWND handle)
		{
			trans = D2D1::Matrix3x2F::Translation(0, 0);
			CurrentTrans = D2D1::Matrix3x2F::Translation(0, 0);
			HRESULT hr;
			Handle = handle;
			RECT rect;
			if (!GetClientRect(handle, &rect)) return false;

			hr = D2D1CreateFactory(D2D1_FACTORY_TYPE_SINGLE_THREADED, &factory);
			if (SUCCEEDED(hr))
				hr = factory->CreateHwndRenderTarget(D2D1::RenderTargetProperties(),
					D2D1::HwndRenderTargetProperties(handle, D2D1::SizeU(rect.right - rect.left,
						rect.bottom - rect.top)), &target);
			else
				return false;
			if (SUCCEEDED(hr))
			{
				hr = DWriteCreateFactory(
					DWRITE_FACTORY_TYPE_SHARED,
					__uuidof(IDWriteFactory),
					reinterpret_cast<IUnknown **>(&m_pDWriteFactory)
				);
				target->CreateSolidColorBrush(D2D1::ColorF(D2D1::ColorF::Black, 1.0f),
					&m_pBlackBrush
				);
				target->CreateSolidColorBrush(D2D1::ColorF(D2D1::ColorF::LightGray, 1.0f),
					&m_pLightGrayBrush
				);
				target->GetTransform(&trans);
				hr = m_pDWriteFactory->CreateTextFormat(
					L"Verdana",
					NULL,
					DWRITE_FONT_WEIGHT_NORMAL,
					DWRITE_FONT_STYLE_NORMAL,
					DWRITE_FONT_STRETCH_NORMAL,
					10,
					L"", //locale
					&m_pTextFormat
				);
				hr = m_pDWriteFactory->CreateTextFormat(
					L"Verdana",
					NULL,
					DWRITE_FONT_WEIGHT_NORMAL,
					DWRITE_FONT_STYLE_NORMAL,
					DWRITE_FONT_STRETCH_NORMAL,
					15,
					L"", //locale
					&m_pTextFormat15
				);
			}
				
			else
				return false;
			if (SUCCEEDED(hr))
				return true;
			else
				return false;

			
		}

		void GetTextLayout(WCHAR text[], FLOAT msc_fontSize)
		{
			HRESULT hr;
			static const WCHAR msc_fontName[] = L"Verdana";
			D2D1_SIZE_F renderTargetSize = target->GetSize();
			
			hr = m_pDWriteFactory->CreateTextLayout(text, wcslen(text), m_pTextFormat, m_pTextFormat->GetFontSize()*wcslen(text), m_pTextFormat->GetFontSize(), &m_pTextLayout);
			
			if (FAILED(hr))
				GetTextLayout(text, msc_fontSize);
			
		}
		
		WCHAR* Reverse(WCHAR str[])
		{
			auto res = str;
			int k = 0;
			for (int i = 0; i < wcslen(str); i++)
			{
				k = wcslen(str) - i - 1;
				res[i] = str[k];
			}
			return res;
		}

		void GetTextLayoutReverse(WCHAR text[], FLOAT msc_fontSize)
		{
			static const WCHAR msc_fontName[] = L"Verdana";
			D2D1_SIZE_F renderTargetSize = target->GetSize();
			m_pDWriteFactory->CreateTextFormat(
				msc_fontName,
				NULL,
				DWRITE_FONT_WEIGHT_NORMAL,
				DWRITE_FONT_STYLE_NORMAL,
				DWRITE_FONT_STRETCH_NORMAL,
				msc_fontSize,
				L"", //locale
				&m_pTextFormat
			);
			m_pTextFormat->SetReadingDirection(DWRITE_READING_DIRECTION_RIGHT_TO_LEFT);
			//text = Reverse(text);
			m_pDWriteFactory->CreateTextLayout(text, wcslen(text), m_pTextFormat, m_pTextFormat->GetFontSize()*wcslen(text), m_pTextFormat->GetFontSize(), &m_pTextLayout);
		}

		void ZoomIn()
		{
			if (k < 6)
			{
				RECT rect;
				GetClientRect(Handle, &rect);

				POINT point;
				getClientPos(&point);
				POINTF pointLocalOld;
				pointLocalOld.x = point.x*pow(10, k) / valOfDivision;
				pointLocalOld.y = point.y*pow(10, k) / valOfDivision;


				valOfDivision -= 10;

				LONG min = min(rect.right, rect.bottom);

				if (min / valOfDivision >= 20)
				{
					valOfDivision = valOfDivision * 10;
					k++;
				}

				else
					if (min / valOfDivision < 2)
					{
						valOfDivision = valOfDivision / 10;
						k--;
					}


				getClientPos(&point);
				POINTF pointLocalNew;
				pointLocalNew.x = point.x*pow(10, k) / valOfDivision;
				pointLocalNew.y = point.y*pow(10, k) / valOfDivision;

				POINTF move;
				move.x = (int)((pointLocalNew.x - pointLocalOld.x)*valOfDivision / pow(10, k));
				move.y = (int)((pointLocalNew.y - pointLocalOld.y)*valOfDivision / pow(10, k));


				moveCamera(move.x, move.y);

			}
				
			//moveCamera(0, 0);
		}
		
		void ZoomOut()
		{
			if (k > -3)
			{
				RECT rect;
				GetClientRect(Handle, &rect);
				
				POINT point;
				getClientPos(&point);
				POINTF pointLocalOld;
				pointLocalOld.x = point.x*pow(10, k) / valOfDivision;
				pointLocalOld.y = point.y*pow(10, k) / valOfDivision;
				

				valOfDivision += 10;

				LONG min = min(rect.right, rect.bottom);

				if (min / valOfDivision >= 20)
				{
					valOfDivision = valOfDivision * 10;
					k++;
				}

				else
					if (min / valOfDivision < 2)
					{
						valOfDivision = valOfDivision / 10;
						k--;
					}


				getClientPos(&point);
				POINTF pointLocalNew;
				pointLocalNew.x = point.x*pow(10, k) / valOfDivision;
				pointLocalNew.y = point.y*pow(10, k) / valOfDivision;

				POINTF move;
				move.x = (int)((pointLocalNew.x - pointLocalOld.x)*valOfDivision/ pow(10, k));
				move.y = (int)((pointLocalNew.y- pointLocalOld.y)*valOfDivision/pow(10, k));


				moveCamera(move.x, move.y);
				//moveCamera((CurrentTrans._31   -point.x)/10, (CurrentTrans._32 -point.y)/10);
			}
				
			//moveCamera(0, 0);
		}

		void CreateText(POINT pos, WCHAR text[], FLOAT msc_fontSize)
		{
			HRESULT hr;
			D2D1_SIZE_F renderTargetSize = target->GetSize();

			//if (FAILED(hr))
				//CreateText(pos);
			if(msc_fontSize==15)
				target->DrawText(
					text,
					wcslen(text),
					m_pTextFormat15,
					D2D1::RectF(pos.x, pos.y, pos.x + renderTargetSize.width, pos.y + renderTargetSize.height),
					m_pBlackBrush
				);
			else
				target->DrawText(
					text,
					wcslen(text),
					m_pTextFormat,
					D2D1::RectF(pos.x, pos.y, pos.x + renderTargetSize.width, pos.y + renderTargetSize.height),
					m_pBlackBrush
					 );



			//target->DrawTextLayout(D2D1::Point2F(pos.x, pos.y), m_pTextLayout, m_pBlackBrush, D2D1_DRAW_TEXT_OPTIONS_NONE);
			
			//delete &hr;
			//delete &m_pTextMetrics;
		}

		D2D1::Matrix3x2F MXSUM(D2D1::Matrix3x2F a, D2D1::Matrix3x2F b)
		{
			
			D2D1::Matrix3x2F c;
			c._11 = a._11;
			c._12 = a._12;
			c._21 = a._21;
			c._22 = a._22;
			c._31 = a._31+b._31;
			c._32 = a._32 + b._32;
			return c;
		}
		
		public: 

		D2D1::Matrix3x2F trans;
		D2D1::Matrix3x2F CurrentTrans;




		void moveCamera(float x, float y)
		{
			//trans = D2D1::Matrix3x2F::Translation(x, y);
			trans._31 = trans._31 + x;
			trans._32 = trans._32 + y;
			//Render(D2D1::ColorF::White);
		}

		int valOfDivision = 100;
		int currentValOfDivision = 100;


		void DrawGrid()
		{

			//if (ScreenToClient(Handle, mPoint))
			//	mPoint->x = mPoint->x;
			RECT rect;
			GetClientRect(Handle, &rect);
			float transX = CurrentTrans._31;
			float transY = CurrentTrans._32;

							

			for (int i = ((int)((transY - rect.bottom)/ currentValOfDivision))*currentValOfDivision; i <= transY; i += currentValOfDivision)
			{
				
				if (i == 0)
					continue;
				target->DrawLine(D2D1::Point2F(-transX, -i), D2D1::Point2F(-transX+rect.right, -i), m_pLightGrayBrush);
				target->DrawLine(D2D1::Point2F(-5, -i), D2D1::Point2F(5, -i), m_pBlackBrush);


				double val = (i* pow(10, currentK)) / currentValOfDivision;
				if (currentK < 0)
					val = round(val*pow(10, -currentK))*pow(10, currentK);
				auto str = std::to_string(  val);

				str.erase(str.find_last_not_of('0') + 1, std::string::npos);
				if (str[str.length() - 1] == '.')
					str = str.substr(0, str.length() - 1);
				std::wstring widestr = std::wstring(str.begin(), str.end());
				POINT p;
				//GetTextLayout((WCHAR*)widestr.c_str(), 10);
				p.x = -10 -7*(str.length());
				p.y = -i-7;
				CreateText( p, (WCHAR*)widestr.c_str(), 10);

			}
			for (int i = ((int)((transX - rect.right) / currentValOfDivision))*currentValOfDivision; i <= transX; i += currentValOfDivision)
			{
				
				if (i == 0)
					continue;
				target->DrawLine(D2D1::Point2F(-i, -transY + rect.bottom), D2D1::Point2F(-i, -transY), m_pLightGrayBrush);
				target->DrawLine(D2D1::Point2F(-i, 5), D2D1::Point2F(-i, -5), m_pBlackBrush);

				double val = -(i* pow(10, currentK)) / currentValOfDivision;
				if (k < 0)
					val = round(val*pow(10, -currentK))*pow(10, currentK);
				auto str = std::to_string(val);
				str.erase(str.find_last_not_of('0') + 1, std::string::npos);
				if (str[str.length() - 1] == '.')
					str = str.substr(0, str.length() - 1);
				std::wstring widestr = std::wstring(str.begin(), str.end());
				POINT p;
				p.x = -i-4 * (str.length());
				p.y = 10;
				//GetTextLayout((WCHAR*)widestr.c_str(), 10);

				CreateText(p, (WCHAR*)widestr.c_str(), 10);
			}

		}

		bool isInit = false;

		void getClientPos(POINT *p)
		{
			GetCursorPos(p);
			float transX = CurrentTrans._31;
			float transY = CurrentTrans._32;
			ScreenToClient(Handle, p);
			


			p->x = p->x - transX;
			p->y = p->y - transY;
		}


		//main function
		void Render()
		{

			

			HRESULT hr;
			if (!target) return;
			RECT rect;
			GetClientRect(Handle, &rect);
			if (!isInit)
			{
				trans._31 = rect.right / 2;
				trans._32 = rect.bottom / 2;
				isInit = true;
			}
			
			


			target->BeginDraw();
			D2D1::Matrix3x2F oldTrans;
			target->GetTransform(&oldTrans);
			


			trans = MXSUM(trans, oldTrans);
			target->SetTransform(trans);

			target->GetTransform(&CurrentTrans);
			POINT point;
			getClientPos(&point);

			float transX = CurrentTrans._31;
			float transY = CurrentTrans._32;

			
			currentValOfDivision = valOfDivision;
			currentK = k;


			target->SetAntialiasMode(D2D1_ANTIALIAS_MODE::D2D1_ANTIALIAS_MODE_ALIASED);

			//ID2D1Bitmap *bitmap;
			//ID2D1BitmapRenderTarget *bitmapRenderTarget;
			//ID2D1DeviceContext *deviceContext;
			//ID2D1Effect *gaussianBlur;
			//ID2D1SolidColorBrush *solidColorBrush;

			
			


			target->Clear(D2D1::ColorF(D2D1::ColorF::White, 1.0f));


			//target->CreateCompatibleRenderTarget(&bitmapRenderTarget);
			//bitmapRenderTarget->CreateSolidColorBrush(D2D1::ColorF(0.0f, 0.6f, 1.0f), &solidColorBrush);
			//bitmapRenderTarget->FillEllipse(D2D1::Ellipse(D2D1::Point2F(0, 0), 100.0f, 100.0f), solidColorBrush);
			//target->QueryInterface(&deviceContext);
			//deviceContext->CreateEffect(CLSID_D2D1GaussianBlur, &gaussianBlur);
	
			//bitmapRenderTarget->GetBitmap(&bitmap);
			//gaussianBlur->SetInput(0, bitmap);
			//gaussianBlur->SetValue(D2D1_GAUSSIANBLUR_PROP_BORDER_MODE, D2D1_BORDER_MODE_SOFT);
			//gaussianBlur->SetValue(D2D1_GAUSSIANBLUR_PROP_STANDARD_DEVIATION, 5.0f);

			//bitmap->
			
			//deviceContext->DrawImage(
			//	gaussianBlur,
			//	D2D1_INTERPOLATION_MODE_LINEAR);
			//bitmap->Release();
			//bitmapRenderTarget->Release();
			//deviceContext->Release();
			//gaussianBlur->Release();
			//solidColorBrush->Release();

			
			DrawGrid();
			//test getClientPos function
			//target->DrawLine(D2D1::Point2F(0, 0), D2D1::Point2F(point.x, point.y), m_pBlackBrush);





			//draw coordinate lines
			{
				target->DrawLine(D2D1::Point2F(-transX, 0), D2D1::Point2F(-transX + rect.right, 0), m_pBlackBrush);
				target->DrawLine(D2D1::Point2F(0, -transY + rect.bottom), D2D1::Point2F(0, -transY), m_pBlackBrush);
				target->DrawLine(D2D1::Point2F(-transX + rect.right, 0), D2D1::Point2F(-transX + rect.right - 10, 10), m_pBlackBrush);
				target->DrawLine(D2D1::Point2F(-transX + rect.right, 0), D2D1::Point2F(-transX + rect.right - 10, -10), m_pBlackBrush);
				target->DrawLine(D2D1::Point2F(0, -transY), D2D1::Point2F(10, -transY + 10), m_pBlackBrush);
				target->DrawLine(D2D1::Point2F(0, -transY), D2D1::Point2F(-10, -transY + 10), m_pBlackBrush);

				POINT xTag;
				xTag.x = -transX + rect.right - 20;
				xTag.y = -20;
				POINT yTag;
				yTag.x = 10;
				yTag.y = -transY;

				//GetTextLayout(L"x", 15);
				CreateText(xTag, L"x", 15);
				//GetTextLayout(L"y", 15);
				CreateText(yTag, L"y", 15);

			}
			

			DrawFunctions();
			
			/*
			for (int i = 0; i < 6000; i++)
			{
				std::vector<float> f;
				f.push_back(i);
				f.pop_back();
			}
			*/
			//drawSin();
			//drawQuadratic();
			
			
			
			hr = target->EndDraw();
			trans._31 = 0;
			trans._32 = 0;
		}

		void DrawSomeFunc()
		{
			RECT rect;
			GetClientRect(Handle, &rect);
			POINT oldDot;

			float transX = trans._31;
			float transY = trans._32;
			bool isPassed = false;
			for (int x = -transX; x < -transX + rect.right; x++)
			{

				POINT newDot;
				if (!IfODZ(x / valOfDivision))
				{
					isPassed = true;
					continue;
				}
				
				newDot.x = x;
				newDot.y = -someFunc(x/valOfDivision)*valOfDivision;

				if (isPassed)
				{
					oldDot = newDot;
					isPassed = false;
				}

				if (x != -transX)
					target->DrawLine(D2D1::Point2F(oldDot.x, oldDot.y), D2D1::Point2F(newDot.x, newDot.y), m_pBlackBrush);


				oldDot = newDot;



			}
		}

		bool IfODZ(float x)
		{
			try
			{
				float val =  sqrt((1 - x)*(1 + x));
				if (val != val)
					return false;
				else
					return true;
			}
			catch (std::exception e)
			{
				return false;
			}
		}

		float someFunc(float x)
		{

			try
			{
				return sqrt((1-x)*(1+x));
			}
			catch (std::exception e)
			{
				RECT rect;
				
				return someFunc(x-0.005);
			}
		}

		void drawSin()
		{
			RECT rect;
			GetClientRect(Handle, &rect);
			POINT oldDot;


			float transX = CurrentTrans._31;
			float transY = CurrentTrans._32;
			for (int x = -transX; x < -transX + rect.right; x++)
			{
				POINT newDot;
				newDot.x = x;
				newDot.y = -sin((x*pow(10, currentK))/(float)currentValOfDivision)*currentValOfDivision /pow(10, currentK);

				

				if (x != -transX)
					target->DrawLine(D2D1::Point2F(oldDot.x, oldDot.y), D2D1::Point2F(newDot.x, newDot.y), m_pBlackBrush);
				
				
				oldDot = newDot;
					
				
				//delete &newDot;
			}

		}

		void drawQuadratic()
		{
			RECT rect;
			GetClientRect(Handle, &rect);
			POINT oldDot;

			float transX = CurrentTrans._31;
			float transY = CurrentTrans._32;
			for (int x = -transX; x < -transX + rect.right; x++)
			{
				POINT newDot;
				newDot.x = x;
				newDot.y =- (((float)x / currentValOfDivision)*((float)x / currentValOfDivision))*currentValOfDivision*pow(10, currentK);



				if (x != -transX)
					target->DrawLine(D2D1::Point2F(oldDot.x, oldDot.y), D2D1::Point2F(newDot.x, newDot.y), m_pBlackBrush);


				oldDot = newDot;

			}
		}

		void Resize(HWND handle)
		{
			if (!target) return;
			RECT rect;
			if (!GetClientRect(handle, &rect)) return;
			D2D1_SIZE_U size = D2D1::SizeU(rect.right - rect.left, rect.bottom - rect.top);
			POINT oldCenter;

			float transX = CurrentTrans._31;
			float transY = CurrentTrans._32;
			oldCenter.x = transX + rect.right / 2;
			oldCenter.y = transY - rect.bottom / 2;
			target->Resize(size);
			POINT newCenter;
			GetClientRect(handle, &rect);
			newCenter.x = transX + rect.right / 2;
			newCenter.y = transY - rect.bottom / 2;
			moveCamera(-newCenter.x + oldCenter.x, -newCenter.y + oldCenter.y);
		}

	private:

		ID2D1Factory* factory;
		ID2D1HwndRenderTarget* target;
	};

	public ref class Scene
	{
	public:
		float r;
		float g;
		float b;

		List<Function^> functions;

		void ZoomIn()
		{
			renderer->ZoomIn();
		}
		void ZoomOut()
		{
			
			renderer->ZoomOut();
		}

		void AddFunction(Function^ function)
		{
			functions.Add(function);
			auto f = function->F;
			renderer->functions.push_back(f);
			//FunctionC* F = new FunctionC(msclr::interop::marshal_as< std::string >(stringFunction));
			//renderer->functions.push_back(F);
		}
		void RemoveFunction(Function^ function)
		{
			functions.Remove(function);
			//FunctionC* F = new FunctionC(msclr::interop::marshal_as< std::string >(stringFunction));
			for (int i = 0; i < renderer->functions.size(); i++)
				if (renderer->functions[i]->functionString == function->F->functionString)
				{
					renderer->functions.erase(renderer->functions.begin() + i);
					break;
				}

		}


		float GetVOD()
		{
			return renderer->valOfDivision;
		}
		void SetVOD(float val)
		{
			renderer->valOfDivision = val;
		}

		Scene(System::IntPtr handle, float r_, float g_, float b_)
		{
			renderer = new Renderer;
			if (renderer) renderer->Initialize((HWND)handle.ToPointer());
			r = r_;
			g = g_;
			b = b_;
		}

		~Scene()
		{
			delete renderer;
		}

		void Resize(System::IntPtr handle)
		{
			HWND hwnd = (HWND)handle.ToPointer();
			if (renderer) renderer->Resize(hwnd);
		}

		void Draw()
		{
			D2D1::ColorF color = D2D1::ColorF(r,g,b);
			if (renderer) renderer->Render();
		}
		void MoveCamera(float x, float y)
		{
			renderer->moveCamera(x, y);
			
		}

	private:
		
		Renderer* renderer;
	};
}