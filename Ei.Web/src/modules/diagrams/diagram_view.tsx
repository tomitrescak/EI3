import React from "react";

import styled from "@emotion/styled";

interface Props {}

export const Container = styled.div<{ color: string; background: string }>`
  flex: 1;
  svg {
    overflow: visible !important;
  }
  width: 100%;
  height: 100%;
  background-color: ${(p) => p.background};
  background-size: 50px 50px;
  display: flex;
  > * {
    height: 100%;
    min-height: 100%;
    width: 100%;
  }
  background-image: linear-gradient(
      0deg,
      transparent 24%,
      ${(p) => p.color} 25%,
      ${(p) => p.color} 26%,
      transparent 27%,
      transparent 74%,
      ${(p) => p.color} 75%,
      ${(p) => p.color} 76%,
      transparent 77%,
      transparent
    ),
    linear-gradient(
      90deg,
      transparent 24%,
      ${(p) => p.color} 25%,
      ${(p) => p.color} 26%,
      transparent 27%,
      transparent 74%,
      ${(p) => p.color} 75%,
      ${(p) => p.color} 76%,
      transparent 77%,
      transparent
    );

  .linkSelected {
    path {
      stroke: salmon;
    }
  }

  circle {
    opacity: 0.4;
  }
`;

type DiagramProps = {
  name: string;
};

export const DiagramView = (props: React.PropsWithChildren<DiagramProps>) => {
  const containerRef = React.useRef<HTMLDivElement>();

  React.useEffect(() => {
    const svgContainer = containerRef.current;
    const svgImage = containerRef.current.children[0];

    let viewBox = {
      x: 0,
      y: 0,
      w: svgImage.clientWidth,
      h: svgImage.clientHeight,
    };

    if (props.name && localStorage.getItem(props.name)) {
      try {
        viewBox = JSON.parse(localStorage.getItem(props.name));
      } catch {}
    }
    svgImage.setAttribute(
      "viewBox",
      `${viewBox.x} ${viewBox.y} ${viewBox.w} ${viewBox.h}`
    );

    const svgSize = { w: svgImage.clientWidth, h: svgImage.clientHeight };
    var isPanning = false;
    var startPoint = { x: 0, y: 0 };
    var endPoint = { x: 0, y: 0 };
    var scale = 1;

    svgContainer.addEventListener("mousewheel", function (e: any) {
      e.preventDefault();
      var w = viewBox.w;
      var h = viewBox.h;
      var mx = e.offsetX; //mouse x
      var my = e.offsetY;
      var dw = w * Math.sign(e.deltaY) * 0.05;
      var dh = h * Math.sign(e.deltaY) * 0.05;
      var dx = (dw * mx) / svgSize.w;
      var dy = (dh * my) / svgSize.h;
      viewBox = {
        x: viewBox.x + dx,
        y: viewBox.y + dy,
        w: viewBox.w - dw,
        h: viewBox.h - dh,
      };
      scale = svgSize.w / viewBox.w;
      // zoomValue.innerText = `${Math.round(scale * 100) / 100}`;
      svgImage.setAttribute(
        "viewBox",
        `${viewBox.x} ${viewBox.y} ${viewBox.w} ${viewBox.h}`
      );

      if (props.name) {
        localStorage.setItem(props.name, JSON.stringify(viewBox));
      }
    });

    svgContainer.addEventListener("mousedown", function (e) {
      isPanning = true;
      startPoint = { x: e.x, y: e.y };
    });

    svgContainer.addEventListener("mousemove", function (e) {
      if (isPanning) {
        endPoint = { x: e.x, y: e.y };
        var dx = (startPoint.x - endPoint.x) / scale;
        var dy = (startPoint.y - endPoint.y) / scale;
        var movedViewBox = {
          x: viewBox.x + dx,
          y: viewBox.y + dy,
          w: viewBox.w,
          h: viewBox.h,
        };
        svgImage.setAttribute(
          "viewBox",
          `${movedViewBox.x} ${movedViewBox.y} ${movedViewBox.w} ${movedViewBox.h}`
        );

        if (props.name) {
          localStorage.setItem(props.name, JSON.stringify(viewBox));
        }
      }
    });

    svgContainer.addEventListener("mouseup", function (e) {
      if (isPanning) {
        endPoint = { x: e.x, y: e.y };
        var dx = (startPoint.x - endPoint.x) / scale;
        var dy = (startPoint.y - endPoint.y) / scale;
        viewBox = {
          x: viewBox.x + dx,
          y: viewBox.y + dy,
          w: viewBox.w,
          h: viewBox.h,
        };
        svgImage.setAttribute(
          "viewBox",
          `${viewBox.x} ${viewBox.y} ${viewBox.w} ${viewBox.h}`
        );

        if (props.name) {
          localStorage.setItem(props.name, JSON.stringify(viewBox));
        }

        isPanning = false;
      }
    });

    svgContainer.addEventListener("mouseleave", function (e) {
      isPanning = false;
    });
  }, []);

  return (
    <Container
      ref={containerRef}
      background={"rgb(60, 60, 60)"}
      color={"rgba(255,255,255, 0.05)"}
    >
      {props.children}
    </Container>
  );
};
