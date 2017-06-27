#include <d2d1.h>
#include <dwrite.h>
#include <dwrite_1.h>
#include <dwrite_2.h>
#include <array>
#include <wchar.h>
#include <strsafe.h>
#include <string>

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

		void CreateText(WCHAR text[], FLOAT msc_fontSize, POINT pos)
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
			target->DrawText(
				text,
				wcslen(text),
				m_pTextFormat,
				D2D1::RectF(pos.x, pos.y, pos.x+renderTargetSize.width, pos.y+renderTargetSize.height),
				m_pBlackBrush
			);

			//m_pTextFormat->SetTextAlignment(DWRITE_TEXT_ALIGNMENT_CENTER);
			//m_pTextFormat->SetParagraphAlignment(DWRITE_PARAGRAPH_ALIGNMENT_CENTER);

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

		float valOfDivision = 100;

		void DrawGrid()
		{
			RECT rect;
			GetClientRect(Handle, &rect);
			float transX = trans._31;
			float transY = trans._32;
			for (int i = ((int)((transY - rect.bottom)/ valOfDivision))*valOfDivision; i <= transY; i += valOfDivision)
			{
				target->DrawLine(D2D1::Point2F(-transX, -i), D2D1::Point2F(-transX+rect.right, -i), m_pLightGrayBrush);
				target->DrawLine(D2D1::Point2F(-5, -i), D2D1::Point2F(5, -i), m_pBlackBrush);
			}
			for (int i = ((int)((transX - rect.right) / valOfDivision))*valOfDivision; i <= transX; i += valOfDivision)
			{
				target->DrawLine(D2D1::Point2F(-i, -transY + rect.bottom), D2D1::Point2F(-i, -transY), m_pLightGrayBrush);
				target->DrawLine(D2D1::Point2F(-i, 5), D2D1::Point2F(-i, -5), m_pBlackBrush);
			}
		}
		bool isInit = false;
		void Render(D2D1::ColorF color)
		{
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

				CreateText(L"x", 15, xTag);
				CreateText(L"y", 15, yTag);
			}
			drawSin();
			//drawProp();
			//CreateText(L"Hello");
			/*
			auto str = std::to_string(rect.bottom);
			std::wstring widestr = std::wstring(str.begin(), str.end());

			CreateText((WCHAR*)widestr.c_str());
			*/
			
			
			
			target->EndDraw();

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
				newDot.y = -sin(x/valOfDivision)*valOfDivision;

				

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
		UINT32 rgb_;
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