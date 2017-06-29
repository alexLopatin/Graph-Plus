#include <d2d1.h>
#include <dwrite.h>
#include <dwrite_1.h>
#include <dwrite_2.h>
#include <array>
#include <wchar.h>
#include <strsafe.h>
#include <string>
#include <math.h>


#pragma comment(lib, "Dwrite")


namespace lib
{
	class Renderer
	{
	public:
		HWND Handle;

		~Renderer()
		{
			if (factory) factory->Release();
			if (target) target->Release();
		}
		
		ID2D1SolidColorBrush *m_pBlackBrush;
		ID2D1SolidColorBrush *m_pLightGrayBrush;
		IDWriteTextFormat *m_pTextFormat;
		IDWriteFactory  *m_pDWriteFactory;
		IDWriteTextLayout *m_pTextLayout;
		int k = 0;
		bool Initialize(HWND handle)
		{
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

			hr = m_pDWriteFactory->CreateTextFormat(
				msc_fontName,
				NULL,
				DWRITE_FONT_WEIGHT_NORMAL,
				DWRITE_FONT_STYLE_NORMAL,
				DWRITE_FONT_STRETCH_NORMAL,
				msc_fontSize,
				L"", //locale
				&m_pTextFormat
			);
			if (FAILED(hr))
				GetTextLayout(text, msc_fontSize);

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
			if(k<6)
				valOfDivision-=10;
			//moveCamera(0, 0);
		}
		void ZoomOut()
		{
			if (k>-3)
				valOfDivision+= 10;
			//moveCamera(0, 0);
		}


		void CreateText(POINT pos)
		{
			HRESULT hr;
			
			DWRITE_TEXT_METRICS m_pTextMetrics;
			hr = m_pTextLayout->GetMetrics(&m_pTextMetrics);
			if (FAILED(hr))
				CreateText(pos);
			target->DrawTextLayout(D2D1::Point2F(pos.x, pos.y), m_pTextLayout, m_pBlackBrush, D2D1_DRAW_TEXT_OPTIONS_NONE);
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
		void moveCamera(float x, float y)
		{
			trans = D2D1::Matrix3x2F::Translation(x, y);

			Render(D2D1::ColorF::White);
		}
		
		bool stopRender;

		int valOfDivision = 100;

		void DrawGrid()
		{
			RECT rect;
			GetClientRect(Handle, &rect);
			float transX = trans._31;
			float transY = trans._32;

			LONG min = min(rect.right, rect.bottom);

			if (min / valOfDivision >= 20)
			{
				valOfDivision = valOfDivision*10;
				k++;
			}
			
			else 
				if (min / valOfDivision < 2)
				{
					valOfDivision = valOfDivision / 10;
					k--;
				}
			
			
				

			for (int i = ((int)((transY - rect.bottom)/ valOfDivision))*valOfDivision; i <= transY; i += valOfDivision)
			{
				if (i == 0)
					continue;
				target->DrawLine(D2D1::Point2F(-transX, -i), D2D1::Point2F(-transX+rect.right, -i), m_pLightGrayBrush);
				target->DrawLine(D2D1::Point2F(-5, -i), D2D1::Point2F(5, -i), m_pBlackBrush);
				double val = (i* pow(10, k)) / valOfDivision;
				if (k < 0)
					val = (int)(val*pow(10, -k))*pow(10, k);
				auto str = std::to_string(  val);

				str.erase(str.find_last_not_of('0') + 1, std::string::npos);
				if (str[str.length() - 1] == '.')
					str = str.substr(0, str.length() - 1);
				std::wstring widestr = std::wstring(str.begin(), str.end());
				POINT p;
				D2D1_SIZE_F renderTargetSize = target->GetSize();

				GetTextLayout((WCHAR*)widestr.c_str(), 10);

				//DWRITE_TEXT_METRICS metrics;
				//m_pTextLayout->GetMetrics(&metrics);
				
				p.x = -10 -7*(str.length());
				p.y = -i-7;
				CreateText( p);
				
			}
			for (int i = ((int)((transX - rect.right) / valOfDivision))*valOfDivision; i <= transX; i += valOfDivision)
			{
				if (i == 0)
					continue;
				target->DrawLine(D2D1::Point2F(-i, -transY + rect.bottom), D2D1::Point2F(-i, -transY), m_pLightGrayBrush);
				target->DrawLine(D2D1::Point2F(-i, 5), D2D1::Point2F(-i, -5), m_pBlackBrush);
				double val = -(i* pow(10, k)) / valOfDivision;
				if (k < 0)
					val = (int)(val*pow(10, -k))*pow(10, k);
				auto str = std::to_string(val);
				str.erase(str.find_last_not_of('0') + 1, std::string::npos);
				if (str[str.length() - 1] == '.')
					str = str.substr(0, str.length() - 1);
				std::wstring widestr = std::wstring(str.begin(), str.end());
				POINT p;
				p.x = -i-4 * (str.length());
				p.y = 10;
				GetTextLayout((WCHAR*)widestr.c_str(), 10);
				CreateText(p);
			}
		}

		

		bool isInit = false;
		void Render(D2D1::ColorF color)
		{
			HRESULT hr;
			stopRender = true;
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


			float transX = trans._31;
			float transY = trans._32;
			stopRender = false;

			target->SetAntialiasMode(D2D1_ANTIALIAS_MODE::D2D1_ANTIALIAS_MODE_ALIASED);
			target->Clear(D2D1::ColorF(color));
			DrawGrid();
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

				GetTextLayout(L"x", 15);
				CreateText(xTag);
				GetTextLayout(L"y", 15);
				CreateText(yTag);
			}
			drawSin();
			drawQuadratic();
			//DrawSomeFunc();
			//drawProp();
			//CreateText(L"Hello");
			/*
			auto str = std::to_string(rect.bottom);
			std::wstring widestr = std::wstring(str.begin(), str.end());

			CreateText((WCHAR*)widestr.c_str());
			*/
			
			
			
			hr = target->EndDraw();
			if (FAILED(hr))
				Render(color);
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
				if (stopRender)
					return;
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

			float transX = trans._31;
			float transY = trans._32;
			for (int x = -transX; x < -transX + rect.right; x++)
			{
				if (stopRender)
					return;
				POINT newDot;
				newDot.x = x;
				newDot.y = -sin((x*pow(10, k))/(float)valOfDivision)*valOfDivision/pow(10, k);

				

				if (x != -transX)
					target->DrawLine(D2D1::Point2F(oldDot.x, oldDot.y), D2D1::Point2F(newDot.x, newDot.y), m_pBlackBrush);
				
				
				oldDot = newDot;
					
					
				
			}
		}

		void drawQuadratic()
		{
			RECT rect;
			GetClientRect(Handle, &rect);
			POINT oldDot;

			float transX = trans._31;
			float transY = trans._32;
			for (int x = -transX; x < -transX + rect.right; x++)
			{
				if (stopRender)
					return;
				POINT newDot;
				newDot.x = x;
				newDot.y =- (((float)x / valOfDivision)*((float)x / valOfDivision))*valOfDivision*pow(10,k);



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
			target->Resize(size);
			
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

		void ZoomIn()
		{
			renderer->ZoomIn();
		}
		void ZoomOut()
		{
			renderer->ZoomOut();
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
			if (renderer) renderer->Render(color);
		}
		void MoveCamera(float x, float y)
		{
			renderer->moveCamera(x, y);
		}

	private:
		
		Renderer* renderer;
	};
}